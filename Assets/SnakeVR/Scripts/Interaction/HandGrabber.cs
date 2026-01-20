using UnityEngine;
using UnityEngine.XR;
using XRInputDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages = UnityEngine.XR.CommonUsages;

namespace SnakeVR
{
    public class HandGrabber : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private XRNode controllerNode = XRNode.RightHand;
        [SerializeField] private float grabRadius = 0.15f;
        [SerializeField] private float gripThreshold = 0.5f;

        private XRInputDevice controller;
        private GrabbableFood heldFood;
        private bool wasGripping = false;

        // For velocity calculation
        private Vector3 previousPosition;
        private Vector3 currentVelocity;

        private void Start()
        {
            controller = InputDevices.GetDeviceAtXRNode(controllerNode);
            previousPosition = transform.position;
        }

        private void Update()
        {
            // Refresh controller if not valid
            if (!controller.isValid)
            {
                controller = InputDevices.GetDeviceAtXRNode(controllerNode);
            }

            // Calculate velocity
            currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
            previousPosition = transform.position;

            // Get grip value
            float gripValue = GetGripValue();
            bool isGripping = gripValue > gripThreshold;

            // Grip just pressed
            if (isGripping && !wasGripping)
            {
                TryGrab();
            }
            // Grip just released
            else if (!isGripping && wasGripping)
            {
                TryRelease();
            }

            wasGripping = isGripping;
        }

        private float GetGripValue()
        {
            float gripValue = 0f;

            if (controller.isValid)
            {
                controller.TryGetFeatureValue(XRCommonUsages.grip, out gripValue);
            }

            // Keyboard fallback for testing (G key)
            if (gripValue == 0f && UnityEngine.InputSystem.Keyboard.current != null)
            {
                if (UnityEngine.InputSystem.Keyboard.current.gKey.isPressed)
                {
                    gripValue = 1f;
                }
            }

            return gripValue;
        }

        private void TryGrab()
        {
            if (heldFood != null) return;

            // Find food in range
            Collider[] colliders = Physics.OverlapSphere(transform.position, grabRadius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Food"))
                {
                    GrabbableFood food = col.GetComponent<GrabbableFood>();
                    if (food != null && food.CanBeGrabbed())
                    {
                        food.Grab(transform);
                        heldFood = food;
                        Debug.Log($"Hand grabbed food: {controllerNode}");
                        return;
                    }
                }
            }
        }

        private void TryRelease()
        {
            if (heldFood != null)
            {
                heldFood.Release(currentVelocity);
                heldFood = null;
                Debug.Log($"Hand released food: {controllerNode}");
            }
        }

        // Clear reference if food is destroyed (eaten)
        private void LateUpdate()
        {
            if (heldFood == null) return;

            // Check if food was destroyed
            if (heldFood.gameObject == null)
            {
                heldFood = null;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, grabRadius);
        }
    }
}
