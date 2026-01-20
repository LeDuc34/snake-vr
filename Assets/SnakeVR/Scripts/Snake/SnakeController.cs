using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeVR
{
    /// <summary>
    /// Controls the snake movement.
    /// IMPORTANT: Attach this script to the XR Origin (XR Rig) GameObject.
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

        // Snake body
        private List<SnakeSegment> segments = new List<SnakeSegment>();

        // Path tracking for segment following
        private struct PathPoint
        {
            public Vector3 position;
            public Quaternion rotation;
            public float distance; // cumulative distance from start
        }
        private List<PathPoint> path = new List<PathPoint>();
        private float totalDistance = 0f;

        // Movement
        private Vector3 currentDirection = Vector3.forward;
        private Vector3 targetDirection = Vector3.forward;

        // Input
        private VRInputManager inputManager;
        private bool inputProcessed = false; // Pour détecter les nouveaux inputs seulement

        // The head is this transform (XR Origin)
        private Transform headTransform;

        // Effect modifiers
        private float speedMultiplier = 1f;
        private bool isGhostMode = false;

#if UNITY_EDITOR
        // Dev mode: press P to freeze snake movement for testing
        private bool devModeFreeze = false;
#endif

        private void Awake()
        {
            // The XR Origin itself is the snake head
            headTransform = transform;
        }

        private void Start()
        {
            inputManager = FindObjectOfType<VRInputManager>();
            ResetSnake();
        }

        private void Update()
        {
            if (GameManager.Instance != null && GameManager.Instance.GetCurrentState() != GameState.Playing)
            {
                return;
            }

#if UNITY_EDITOR
            // Dev mode: P to toggle freeze
            if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
            {
                devModeFreeze = !devModeFreeze;
                Debug.Log($"[DEV] Snake movement {(devModeFreeze ? "FROZEN" : "RESUMED")} - Press P to toggle");
            }

            if (devModeFreeze)
            {
                return; // Skip movement, allow testing grab mechanics
            }
#endif

            HandleInput();
            MoveSnake();
        }

        private void HandleInput()
        {
            if (inputManager != null)
            {
                Vector2 input = inputManager.GetMovementInput();

                // Reset flag when joystick returns to center
                if (input.magnitude < 0.3f)
                {
                    inputProcessed = false;
                    return;
                }

                // Only process input once per joystick movement
                if (inputProcessed)
                {
                    return;
                }

                // Mark as processed to ignore repeated inputs
                inputProcessed = true;

                // Calculate new direction based on current rotation
                Quaternion currentRotation = Quaternion.LookRotation(currentDirection);
                Quaternion newRotation = currentRotation;

                if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
                {
                    // Horizontal input (left/right) - rotate around Y axis
                    if (input.x > 0)
                    {
                        // Right: +90° around Y
                        newRotation = currentRotation * Quaternion.Euler(0, 90, 0);
                    }
                    else
                    {
                        // Left: -90° around Y
                        newRotation = currentRotation * Quaternion.Euler(0, -90, 0);
                    }
                }
                else
                {
                    // Vertical input (up/down) - rotate around X axis
                    if (input.y > 0)
                    {
                        // Up: -90° around X (pitch up)
                        newRotation = currentRotation * Quaternion.Euler(-90, 0, 0);
                    }
                    else
                    {
                        // Down: +90° around X (pitch down)
                        newRotation = currentRotation * Quaternion.Euler(90, 0, 0);
                    }
                }

                // Extract new direction from rotation
                Vector3 newDirection = newRotation * Vector3.forward;

                // Don't allow 180 degree turns (going backwards)
                float dot = Vector3.Dot(newDirection.normalized, currentDirection.normalized);
                if (dot > -0.9f)
                {
                    // Apply rotation IMMEDIATELY
                    currentDirection = newDirection.normalized;
                    targetDirection = newDirection.normalized;
                }
            }
        }

        private void MoveSnake()
        {
            // Smooth rotation towards target direction
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

            // Move continuously in current direction (apply speed multiplier)
            headTransform.position += currentDirection * moveSpeed * speedMultiplier * Time.deltaTime;
            headTransform.rotation = Quaternion.LookRotation(currentDirection);

            // Record path continuously
            float distanceMoved = Vector3.Distance(previousPosition, headTransform.position);
            if (distanceMoved > 0.001f) // avoid duplicates
            {
                totalDistance += distanceMoved;
                path.Add(new PathPoint
                {
                    position = headTransform.position,
                    rotation = headTransform.rotation,
                    distance = totalDistance
                });
            }

            // Clean up old path points
            CleanupPath();

            // Update segments every frame
            UpdateSegments();
        }

        private void UpdateSegments()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                // Each segment is at a fixed distance behind the previous one
                float targetDistance = totalDistance - (i + 1) * segmentSpacing;

                if (targetDistance < 0) continue; // not enough path recorded yet

                // Find position on path
                (Vector3 pos, Quaternion rot) = GetPositionOnPath(targetDistance);
                segments[i].SetPositionAndRotation(pos, rot);
            }
        }

        private (Vector3, Quaternion) GetPositionOnPath(float targetDistance)
        {
            // Find the two points to interpolate between
            for (int i = path.Count - 1; i > 0; i--)
            {
                if (path[i].distance >= targetDistance && path[i - 1].distance <= targetDistance)
                {
                    // Interpolate between path[i-1] and path[i]
                    float segmentLength = path[i].distance - path[i - 1].distance;
                    if (segmentLength < 0.0001f) continue;

                    float t = (targetDistance - path[i - 1].distance) / segmentLength;
                    return (
                        Vector3.Lerp(path[i - 1].position, path[i].position, t),
                        Quaternion.Slerp(path[i - 1].rotation, path[i].rotation, t)
                    );
                }
            }

            // Fallback: last known point
            if (path.Count > 0)
                return (path[path.Count - 1].position, path[path.Count - 1].rotation);

            return (headTransform.position, headTransform.rotation);
        }

        private void CleanupPath()
        {
            // Minimum required distance = last segment position
            float minRequiredDistance = totalDistance - (segments.Count + 2) * segmentSpacing;

            // Remove points that are too old
            while (path.Count > 0 && path[0].distance < minRequiredDistance)
            {
                path.RemoveAt(0);
            }
        }

        public void ResetSnake()
        {
            // Clear existing segments
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

            // Reset effect modifiers
            speedMultiplier = 1f;
            isGhostMode = false;

            // Reset position and direction - Start above ground level
            headTransform.position = new Vector3(0, 1.5f, 0);
            currentDirection = Vector3.forward;
            targetDirection = Vector3.forward;
            headTransform.rotation = Quaternion.LookRotation(currentDirection);

            // Initialize path with starting position
            path.Add(new PathPoint
            {
                position = headTransform.position,
                rotation = headTransform.rotation,
                distance = 0f
            });

            // Create initial segments
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
                // Create default cube if no prefab
                segmentObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                segmentObj.transform.localScale = Vector3.one * 0.25f;
                segmentObj.transform.SetParent(transform);
            }

            SnakeSegment segment = segmentObj.GetComponent<SnakeSegment>();
            if (segment == null)
            {
                segment = segmentObj.AddComponent<SnakeSegment>();
            }

            // Position segment behind the last segment
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

            // Notify UI of length change
            if (UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.UpdateSnakeLength(segments.Count + 1);
            }

            Debug.Log($"Snake length: {segments.Count + 1}");
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

            Debug.Log($"Removed {toRemove} segments. Snake length: {segments.Count + 1}");
        }

        public void SetSpeed(float speed)
        {
            moveSpeed = speed;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            speedMultiplier = multiplier;
            Debug.Log($"Speed multiplier set to {multiplier}");
        }

        public void SetGhostMode(bool enabled)
        {
            isGhostMode = enabled;
            Debug.Log($"Ghost mode: {enabled}");
        }

        public bool IsGhostMode()
        {
            return isGhostMode;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Food is now handled by GrabbableFood + HandGrabber
            // Only handle game over conditions here

            if (other.CompareTag("Wall"))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameOver();
                }
            }
            else if (other.CompareTag("SnakeBody"))
            {
                // Ghost mode allows passing through own body
                if (!isGhostMode)
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.GameOver();
                    }
                }
            }
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
    }
}
