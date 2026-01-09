using System.Collections.Generic;
using UnityEngine;

namespace SnakeVR
{
    public class SnakeController : MonoBehaviour
    {
        [Header("Snake Settings")]
        [SerializeField] private GameObject segmentPrefab;
        [SerializeField] private float segmentSpacing = 0.3f;
        [SerializeField] private int initialSegmentCount = 3;
        [SerializeField] private Transform headTransform;

        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 2f;
        [SerializeField] private float turnSpeed = 90f;

        // Snake body
        private List<SnakeSegment> segments = new List<SnakeSegment>();
        private List<Vector3> positionHistory = new List<Vector3>();

        // Movement
        private Vector3 currentDirection = Vector3.forward;
        private Vector3 targetDirection = Vector3.forward;
        private float moveTimer = 0f;
        private float moveInterval = 0.5f;

        // Input
        private VRInputManager inputManager;

        private void Awake()
        {
            if (headTransform == null)
            {
                headTransform = transform;
            }
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

            HandleInput();
            MoveSnake();
        }

        private void HandleInput()
        {
            if (inputManager != null)
            {
                Vector2 input = inputManager.GetMovementInput();

                if (input.magnitude > 0.3f)
                {
                    // Convert 2D input to 3D direction
                    Vector3 inputDirection = new Vector3(input.x, 0, input.y).normalized;

                    // Don't allow 180 degree turns
                    if (Vector3.Dot(inputDirection, -currentDirection) < 0.9f)
                    {
                        targetDirection = inputDirection;
                    }
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

            // Move at intervals
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveInterval)
            {
                moveTimer = 0f;
                MoveStep();
            }
        }

        private void MoveStep()
        {
            // Store current head position
            positionHistory.Insert(0, headTransform.position);

            // Move head
            headTransform.position += currentDirection * segmentSpacing;
            headTransform.rotation = Quaternion.LookRotation(currentDirection);

            // Limit history size
            if (positionHistory.Count > segments.Count * 2)
            {
                positionHistory.RemoveAt(positionHistory.Count - 1);
            }

            // Update segments
            UpdateSegments();
        }

        private void UpdateSegments()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                int historyIndex = (i + 1) * Mathf.RoundToInt(segmentSpacing / segmentSpacing);

                if (historyIndex < positionHistory.Count)
                {
                    Vector3 targetPosition = positionHistory[historyIndex];
                    segments[i].MoveTo(targetPosition);

                    // Rotation
                    if (i == 0)
                    {
                        Vector3 direction = (headTransform.position - targetPosition).normalized;
                        segments[i].transform.rotation = Quaternion.LookRotation(direction);
                    }
                    else if (historyIndex + 1 < positionHistory.Count)
                    {
                        Vector3 direction = (positionHistory[historyIndex - 1] - positionHistory[historyIndex + 1]).normalized;
                        if (direction != Vector3.zero)
                        {
                            segments[i].transform.rotation = Quaternion.LookRotation(direction);
                        }
                    }
                }
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
            positionHistory.Clear();

            // Reset position and direction - Start above ground level
            headTransform.position = new Vector3(0, 1.5f, 0);
            currentDirection = Vector3.forward;
            targetDirection = Vector3.forward;
            headTransform.rotation = Quaternion.LookRotation(currentDirection);

            // Create initial segments
            for (int i = 0; i < initialSegmentCount; i++)
            {
                AddSegment();
            }

            moveTimer = 0f;
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

            Debug.Log($"Snake length: {segments.Count + 1}");
        }

        public void SetSpeed(float speed)
        {
            moveSpeed = speed;
            moveInterval = 1f / speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Food"))
            {
                // Eat food
                AddSegment();
                Destroy(other.gameObject);

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnFoodEaten();
                }
            }
            else if (other.CompareTag("Wall") || other.CompareTag("SnakeBody"))
            {
                // Game Over
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.GameOver();
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
    }
}
