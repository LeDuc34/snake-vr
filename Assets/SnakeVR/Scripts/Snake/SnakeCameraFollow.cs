    using UnityEngine;

namespace SnakeVR
{
    /// <summary>
    /// Makes the VR camera follow the snake head for first-person POV
    /// Attach to the XR Origin
    /// </summary>
    public class SnakeCameraFollow : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform snakeHead;
        [SerializeField] private Vector3 cameraOffset = new Vector3(0, 0.2f, -0.5f);
        [SerializeField] private float followSpeed = 10f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private bool followPosition = true;
        [SerializeField] private bool followRotation = true;

        private void Start()
        {
            // Auto-find snake head if not assigned
            if (snakeHead == null)
            {
                SnakeController snake = FindObjectOfType<SnakeController>();
                if (snake != null)
                {
                    snakeHead = snake.GetHeadTransform();
                }
            }
        }

        private void LateUpdate()
        {
            if (snakeHead == null)
                return;

            // Calculate target position - behind and slightly above the snake head
            Vector3 targetPosition = snakeHead.position + snakeHead.TransformDirection(cameraOffset);

            // Smoothly move to target position
            if (followPosition)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
            }

            // Smoothly rotate to match snake head rotation
            if (followRotation)
            {
                Quaternion targetRotation = snakeHead.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Debug visualization
        private void OnDrawGizmos()
        {
            if (snakeHead != null && Application.isPlaying)
            {
                Gizmos.color = Color.cyan;
                Vector3 targetPosition = snakeHead.position + snakeHead.TransformDirection(cameraOffset);
                Gizmos.DrawWireSphere(targetPosition, 0.1f);
                Gizmos.DrawLine(snakeHead.position, targetPosition);
            }
        }
    }
}
