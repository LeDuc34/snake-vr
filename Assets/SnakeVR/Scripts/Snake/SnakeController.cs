using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeVR
{
    /// <summary>
    /// Controls the snake movement using native XRI Input Actions.
    /// Attach this script to the XR Origin (XR Rig) GameObject.
    /// The XR Origin IS the snake head - the camera will move with the snake!
    /// </summary>
    public class SnakeController : MonoBehaviour
    {
        [Header("Snake Settings")]
        [SerializeField] private GameObject segmentPrefab;
        [SerializeField] private float segmentSpacing = 0.3f;
        [SerializeField] private int initialSegmentCount = 3;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float turnSpeed = 90f;

        [Header("Input - Native XRI")]
        [Tooltip("Reference to XRI Left/Thumbstick or XRI Right/Thumbstick action")]
        [SerializeField] private InputActionReference thumbstickAction;
        [SerializeField] private float joystickDeadzone = 0.3f;

        [Header("Camera Reference")]
        [Tooltip("La caméra VR (Main Camera) pour que les segments suivent sa position")]
        [SerializeField] private Transform vrCamera;

        // Snake body
        private List<SnakeSegment> segments = new List<SnakeSegment>();

        // Path tracking for segment following
        private struct PathPoint
        {
            public Vector3 position;
            public Quaternion rotation;
            public float distance;
        }
        private List<PathPoint> path = new List<PathPoint>();
        private float totalDistance = 0f;

        // Movement
        private Vector3 currentDirection = Vector3.forward;
        private Vector3 targetDirection = Vector3.forward;

        // Input
        private bool inputProcessed = false;

        // The head is this transform (XR Origin)
        private Transform headTransform;

        // Effect modifiers
        private float speedMultiplier = 1f;
        private bool isGhostMode = false;

        private void Awake()
        {
            headTransform = transform;
        }

        private void OnEnable()
        {
            if (thumbstickAction != null && thumbstickAction.action != null)
            {
                thumbstickAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (thumbstickAction != null && thumbstickAction.action != null)
            {
                thumbstickAction.action.Disable();
            }
        }

        private void Start()
        {
            ResetSnake();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.GetCurrentState() != GameState.Playing)
            {
                return;
            }

            HandleInput();
            MoveSnake();
        }

        private void HandleInput()
        {
            Vector2 input = GetThumbstickInput();

            // Reset flag when joystick returns to center
            if (input.magnitude < joystickDeadzone)
            {
                inputProcessed = false;
                return;
            }

            // Only process input once per joystick movement
            if (inputProcessed)
            {
                return;
            }

            inputProcessed = true;

            // Calculate new direction based on current rotation
            Quaternion currentRotation = Quaternion.LookRotation(currentDirection);
            Quaternion newRotation = currentRotation;

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                // Horizontal input (left/right)
                if (input.x > 0)
                {
                    newRotation = currentRotation * Quaternion.Euler(0, 90, 0);
                }
                else
                {
                    newRotation = currentRotation * Quaternion.Euler(0, -90, 0);
                }
            }
            else
            {
                // Vertical input (up/down)
                if (input.y > 0)
                {
                    newRotation = currentRotation * Quaternion.Euler(-90, 0, 0);
                }
                else
                {
                    newRotation = currentRotation * Quaternion.Euler(90, 0, 0);
                }
            }

            Vector3 newDirection = newRotation * Vector3.forward;

            // Don't allow 180 degree turns
            float dot = Vector3.Dot(newDirection.normalized, currentDirection.normalized);
            if (dot > -0.9f)
            {
                currentDirection = newDirection.normalized;
                targetDirection = newDirection.normalized;
            }
        }

        private Vector2 GetThumbstickInput()
        {
            if (thumbstickAction == null || thumbstickAction.action == null)
            {
                return Vector2.zero;
            }

            return thumbstickAction.action.ReadValue<Vector2>();
        }

        private void MoveSnake()
        {
            if (targetDirection != currentDirection)
            {
                currentDirection = Vector3.RotateTowards(
                    currentDirection,
                    targetDirection,
                    turnSpeed * Mathf.Deg2Rad * Time.deltaTime,
                    0f
                );
            }

            Vector3 previousPosition = headTransform.position;

            headTransform.position += currentDirection * moveSpeed * speedMultiplier * Time.deltaTime;
            headTransform.rotation = Quaternion.LookRotation(currentDirection);

            // Utiliser la position de la caméra VR pour le path (les segments suivent la caméra)
            Transform pathReference = vrCamera != null ? vrCamera : headTransform;
            Vector3 pathPosition = pathReference.position;

            float distanceMoved = Vector3.Distance(previousPosition, headTransform.position);
            if (distanceMoved > 0.001f)
            {
                totalDistance += distanceMoved;
                path.Add(new PathPoint
                {
                    position = pathPosition,
                    rotation = headTransform.rotation,
                    distance = totalDistance
                });
            }

            CleanupPath();
            UpdateSegments();
        }

        private void UpdateSegments()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                float targetDistance = totalDistance - (i + 1) * segmentSpacing;
                if (targetDistance < 0) continue;

                (Vector3 pos, Quaternion rot) = GetPositionOnPath(targetDistance);
                segments[i].SetPositionAndRotation(pos, rot);
            }
        }

        private (Vector3, Quaternion) GetPositionOnPath(float targetDistance)
        {
            for (int i = path.Count - 1; i > 0; i--)
            {
                if (path[i].distance >= targetDistance && path[i - 1].distance <= targetDistance)
                {
                    float segmentLength = path[i].distance - path[i - 1].distance;
                    if (segmentLength < 0.0001f) continue;

                    float t = (targetDistance - path[i - 1].distance) / segmentLength;
                    return (
                        Vector3.Lerp(path[i - 1].position, path[i].position, t),
                        Quaternion.Slerp(path[i - 1].rotation, path[i].rotation, t)
                    );
                }
            }

            if (path.Count > 0)
                return (path[path.Count - 1].position, path[path.Count - 1].rotation);

            return (headTransform.position, headTransform.rotation);
        }

        private void CleanupPath()
        {
            float minRequiredDistance = totalDistance - (segments.Count + 2) * segmentSpacing;
            while (path.Count > 0 && path[0].distance < minRequiredDistance)
            {
                path.RemoveAt(0);
            }
        }

        public void ResetSnake()
        {
            foreach (var segment in segments)
            {
                if (segment != null)
                {
                    Destroy(segment.gameObject);
                }
            }
            segments.Clear();
            path.Clear();
            totalDistance = 0f;

            speedMultiplier = 1f;
            isGhostMode = false;

            headTransform.position = new Vector3(0, 0f, 0);
            currentDirection = Vector3.forward;
            targetDirection = Vector3.forward;
            headTransform.rotation = Quaternion.LookRotation(currentDirection);

            // Utiliser la position de la caméra VR pour le path initial
            Transform pathReference = vrCamera != null ? vrCamera : headTransform;
            path.Add(new PathPoint
            {
                position = pathReference.position,
                rotation = headTransform.rotation,
                distance = 0f
            });

            for (int i = 0; i < initialSegmentCount; i++)
            {
                AddSegment();
            }
        }

        public void AddSegment()
        {
            GameObject segmentObj;

            if (segmentPrefab != null)
            {
                segmentObj = Instantiate(segmentPrefab, transform);
            }
            else
            {
                segmentObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segmentObj.transform.localScale = Vector3.one * 0.25f;
                segmentObj.transform.SetParent(transform);
            }

            SnakeSegment segment = segmentObj.GetComponent<SnakeSegment>();
            if (segment == null)
            {
                segment = segmentObj.AddComponent<SnakeSegment>();
            }

            Vector3 position;
            if (segments.Count == 0)
            {
                position = headTransform.position - currentDirection * segmentSpacing;
            }
            else
            {
                SnakeSegment lastSegment = segments[segments.Count - 1];
                position = lastSegment.transform.position - currentDirection * segmentSpacing;
            }

            segmentObj.transform.position = position;
            segments.Add(segment);

            if (UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.UpdateSnakeLength(segments.Count + 1);
            }
        }

        public void AddSegments(int count)
        {
            for (int i = 0; i < count; i++)
            {
                AddSegment();
            }
        }

        public void RemoveSegments(int count)
        {
            int toRemove = Mathf.Min(count, segments.Count);

            for (int i = 0; i < toRemove; i++)
            {
                if (segments.Count > 0)
                {
                    int lastIndex = segments.Count - 1;
                    SnakeSegment segment = segments[lastIndex];
                    segments.RemoveAt(lastIndex);

                    if (segment != null)
                    {
                        Destroy(segment.gameObject);
                    }
                }
            }

            if (UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.UpdateSnakeLength(segments.Count + 1);
            }
        }

        public void SetSpeed(float speed)
        {
            moveSpeed = speed;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            speedMultiplier = multiplier;
        }

        public void SetGhostMode(bool enabled)
        {
            isGhostMode = enabled;
        }

        public bool IsGhostMode()
        {
            return isGhostMode;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"[SnakeController] OnTriggerEnter: {other.name}, tag: {other.tag}");

            if (other.CompareTag("Wall"))
            {
                Debug.Log("[SnakeController] Wall collision detected!");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameOver();
                }
            }
            else if (other.CompareTag("SnakeBody"))
            {
                if (!isGhostMode)
                {
                    Debug.Log("[SnakeController] SnakeBody collision detected!");
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.GameOver();
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"[SnakeController] OnCollisionEnter: {collision.gameObject.name}, tag: {collision.gameObject.tag}");
        }

        public Transform GetHeadTransform()
        {
            return headTransform;
        }

        public List<Vector3> GetOccupiedPositions()
        {
            List<Vector3> positions = new List<Vector3> { headTransform.position };
            foreach (var segment in segments)
            {
                positions.Add(segment.transform.position);
            }
            return positions;
        }

        public int GetSegmentCount()
        {
            return segments.Count;
        }

        public void RepositionToStart()
        {
            headTransform.position = new Vector3(0, 0f, 0);
            headTransform.rotation = Quaternion.LookRotation(Vector3.forward);
        }
    }
}
