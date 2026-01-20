using UnityEngine;

namespace SnakeVR
{
    public class SnakeSegment : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private Color segmentColor = Color.green;
        [SerializeField] private float smoothTime = 0.1f;

        private Vector3 velocity = Vector3.zero;
        private Renderer segmentRenderer;

        private void Awake()
        {
            segmentRenderer = GetComponent<Renderer>();
            if (segmentRenderer == null)
            {
                segmentRenderer = GetComponentInChildren<Renderer>();
            }

            // Add tag for collision detection
            gameObject.tag = "SnakeBody";

            // Setup collider if not present
            Collider col = GetComponent<Collider>();
            if (col == null)
            {
                col = gameObject.AddComponent<BoxCollider>();
            }
            col.isTrigger = true;
        }

        private void Start()
        {
            ApplyColor();
        }

        public void MoveTo(Vector3 targetPosition)
        {
            // Smooth movement (legacy, kept for compatibility)
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref velocity,
                smoothTime
            );
        }

        public void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            // Direct positioning for path-following system
            transform.position = position;
            transform.rotation = rotation;
        }

        public void SetColor(Color color)
        {
            segmentColor = color;
            ApplyColor();
        }

        private void ApplyColor()
        {
            if (segmentRenderer != null)
            {
                if (segmentRenderer.material != null)
                {
                    segmentRenderer.material.color = segmentColor;
                }
            }
        }

        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}
