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

        // Food type for special effects
        private FoodType foodType = FoodType.Normal;

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

        public void SetFoodType(FoodType type)
        {
            foodType = type;
        }

        public FoodType GetFoodType()
        {
            return foodType;
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

            Debug.Log($"Food grabbed: {foodType}");
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
            // Apply special food effect
            if (foodType != FoodType.Normal && SpecialFoodManager.Instance != null)
            {
                SpecialFoodManager.Instance.ApplyEffect(foodType);
            }

            // Get point multiplier for this food type
            float pointMultiplier = 1f;
            if (SpecialFoodManager.Instance != null)
            {
                pointMultiplier = SpecialFoodManager.Instance.GetPointMultiplier(foodType);
            }

            // Notify game systems
            SnakeController snake = FindObjectOfType<SnakeController>();
            if (snake != null)
            {
                // Normal food and most special foods add a segment
                // Shrink and SuperGrowth handle their own segment logic
                if (foodType != FoodType.Shrink && foodType != FoodType.SuperGrowth)
                {
                    snake.AddSegment();
                }
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnFoodEaten(pointMultiplier);

                // Track for point multiplier effect
                if (SpecialFoodManager.Instance != null)
                {
                    SpecialFoodManager.Instance.OnFoodEatenForMultiplier();
                }
            }

            Debug.Log($"Food eaten: {foodType}!");
            Destroy(gameObject);
        }

        public FoodState GetState()
        {
            return currentState;
        }
    }
}
