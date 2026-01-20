using UnityEngine;

namespace SnakeVR
{
    public enum FoodState
    {
        Free,
        Held
    }

    public class GrabbableFood : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float eatDistance = 0.3f;

        private FoodState currentState = FoodState.Free;
        private Transform holdingHand;
        private Rigidbody rb;
        private Collider col;
        private Transform headTransform;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();

            // Find the VR camera (head)
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                headTransform = mainCam.transform;
            }
        }

        private void Update()
        {
            if (currentState == FoodState.Held && holdingHand != null)
            {
                // Follow the hand
                transform.position = holdingHand.position;

                // Check if close to head (eat)
                if (headTransform != null)
                {
                    float distanceToHead = Vector3.Distance(transform.position, headTransform.position);
                    if (distanceToHead < eatDistance)
                    {
                        Eat();
                    }
                }
            }
        }

        public bool CanBeGrabbed()
        {
            return currentState == FoodState.Free;
        }

        public void Grab(Transform hand)
        {
            if (currentState != FoodState.Free) return;

            currentState = FoodState.Held;
            holdingHand = hand;

            // Disable physics while held
            if (rb != null)
            {
                rb.isKinematic = true;
            }
            if (col != null)
            {
                col.enabled = false;
            }

            Debug.Log("Food grabbed");
        }

        public void Release(Vector3 velocity)
        {
            if (currentState != FoodState.Held) return;

            currentState = FoodState.Free;
            holdingHand = null;

            // Re-enable physics
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.linearVelocity = velocity;
            }
            if (col != null)
            {
                col.enabled = true;
            }

            Debug.Log("Food released with velocity: " + velocity);
        }

        private void Eat()
        {
            // Notify game systems
            SnakeController snake = FindObjectOfType<SnakeController>();
            if (snake != null)
            {
                snake.AddSegment();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnFoodEaten();
            }

            Debug.Log("Food eaten!");
            Destroy(gameObject);
        }

        public FoodState GetState()
        {
            return currentState;
        }
    }
}
