using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace SnakeVR
{
    /// <summary>
    /// Food that can be grabbed using XR Interaction Toolkit and eaten by bringing to mouth.
    /// </summary>
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GrabbableFood : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float eatDistance = 0.3f;

        private XRGrabInteractable grabInteractable;
        private Transform headTransform;
        private bool isHeld = false;

        // Food type for special effects
        private FoodType foodType = FoodType.Normal;

        private void Awake()
        {
            // Get or add XRGrabInteractable
            grabInteractable = GetComponent<XRGrabInteractable>();
            if (grabInteractable == null)
            {
                grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
            }

            // Configure grab interactable
            grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;
            grabInteractable.throwOnDetach = true;

            // Find the VR camera (head)
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                headTransform = mainCam.transform;
            }
        }

        private void OnEnable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.AddListener(OnGrabbed);
                grabInteractable.selectExited.AddListener(OnReleased);
            }
        }

        private void OnDisable()
        {
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnGrabbed);
                grabInteractable.selectExited.RemoveListener(OnReleased);
            }
        }

        private void Update()
        {
            if (isHeld && headTransform != null)
            {
                // Check if close to head (eat)
                float distanceToHead = Vector3.Distance(transform.position, headTransform.position);
                if (distanceToHead < eatDistance)
                {
                    Eat();
                }
            }
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            isHeld = true;
            Debug.Log($"Food grabbed: {foodType}");
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            isHeld = false;
            Debug.Log($"Food released: {foodType}");
        }

        public void SetFoodType(FoodType type)
        {
            foodType = type;
        }

        public FoodType GetFoodType()
        {
            return foodType;
        }

        private void Eat()
        {
            // Force release from interactor before destroying
            if (grabInteractable != null && grabInteractable.isSelected)
            {
                // Get the interactor and force deselect
                var interactor = grabInteractable.firstInteractorSelecting;
                if (interactor != null)
                {
                    var interactionManager = grabInteractable.interactionManager;
                    if (interactionManager != null)
                    {
                        interactionManager.SelectExit(interactor, grabInteractable);
                    }
                }
            }

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
            SnakeController snake = FindFirstObjectByType<SnakeController>();
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
    }
}
