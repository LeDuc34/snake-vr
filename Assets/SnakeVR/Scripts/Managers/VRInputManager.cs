using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace SnakeVR
{
    public enum ControlScheme
    {
        LeftJoystick,
        RightJoystick,
        HeadGaze,
        RightControllerDirection
    }

    public class VRInputManager : MonoBehaviour
    {
        [Header("Control Settings")]
        [SerializeField] private ControlScheme controlScheme = ControlScheme.LeftJoystick;
        [SerializeField] private float joystickDeadzone = 0.3f;

        [Header("Head Gaze Settings")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float gazeSmoothing = 5f;

        [Header("Controller References")]
        [SerializeField] private XRNode leftControllerNode = XRNode.LeftHand;
        [SerializeField] private XRNode rightControllerNode = XRNode.RightHand;

        // Input devices
        private InputDevice leftController;
        private InputDevice rightController;

        // Cached input
        private Vector2 currentInput = Vector2.zero;

        private void Start()
        {
            // Find camera if not assigned
            if (cameraTransform == null)
            {
                Camera mainCam = Camera.main;
                if (mainCam != null)
                {
                    cameraTransform = mainCam.transform;
                }
            }

            // Get controllers
            leftController = InputDevices.GetDeviceAtXRNode(leftControllerNode);
            rightController = InputDevices.GetDeviceAtXRNode(rightControllerNode);
        }

        private void Update()
        {
            // Refresh devices if not valid
            if (!leftController.isValid)
            {
                leftController = InputDevices.GetDeviceAtXRNode(leftControllerNode);
            }
            if (!rightController.isValid)
            {
                rightController = InputDevices.GetDeviceAtXRNode(rightControllerNode);
            }

            // Update input based on control scheme
            UpdateInput();

            // Check for pause button (Menu button on either controller)
            if (GetMenuButtonDown())
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PauseGame();
                }
            }

            // Check for start button (A button on right controller)
            if (GetStartButtonDown())
            {
                if (GameManager.Instance != null && GameManager.Instance.GetCurrentState() == GameState.Menu)
                {
                    GameManager.Instance.StartGame();
                }
            }
        }

        private void UpdateInput()
        {
            switch (controlScheme)
            {
                case ControlScheme.LeftJoystick:
                    currentInput = GetJoystickInput(leftController);
                    break;

                case ControlScheme.RightJoystick:
                    currentInput = GetJoystickInput(rightController);
                    break;

                case ControlScheme.HeadGaze:
                    currentInput = GetHeadGazeInput();
                    break;

                case ControlScheme.RightControllerDirection:
                    currentInput = GetControllerDirectionInput(rightController);
                    break;
            }
        }

        private Vector2 GetJoystickInput(InputDevice controller)
        {
            Vector2 joystickValue = Vector2.zero;

            if (controller.isValid)
            {
                // Try primary 2D axis (thumbstick)
                if (controller.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 axis))
                {
                    joystickValue = axis;
                }
            }

            // Apply deadzone
            if (joystickValue.magnitude < joystickDeadzone)
            {
                joystickValue = Vector2.zero;
            }

            return joystickValue;
        }

        private Vector2 GetHeadGazeInput()
        {
            if (cameraTransform == null)
                return Vector2.zero;

            // Convert head direction to 2D input
            Vector3 forward = cameraTransform.forward;
            forward.y = 0; // Ignore vertical component
            forward.Normalize();

            Vector2 input = new Vector2(forward.x, forward.z);

            // Smooth the input
            currentInput = Vector2.Lerp(currentInput, input, Time.deltaTime * gazeSmoothing);

            return currentInput;
        }

        private Vector2 GetControllerDirectionInput(InputDevice controller)
        {
            if (!controller.isValid)
                return Vector2.zero;

            // Get controller rotation
            if (controller.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation))
            {
                Vector3 forward = rotation * Vector3.forward;
                forward.y = 0; // Ignore vertical component
                forward.Normalize();

                return new Vector2(forward.x, forward.z);
            }

            return Vector2.zero;
        }

        public Vector2 GetMovementInput()
        {
            return currentInput;
        }

        private bool GetMenuButtonDown()
        {
            bool leftMenu = false, rightMenu = false;

            if (leftController.isValid)
            {
                leftController.TryGetFeatureValue(CommonUsages.menuButton, out leftMenu);
            }
            if (rightController.isValid)
            {
                rightController.TryGetFeatureValue(CommonUsages.menuButton, out rightMenu);
            }

            return leftMenu || rightMenu;
        }

        private bool GetStartButtonDown()
        {
            bool pressed = false;

            if (rightController.isValid)
            {
                // Try primary button (A button on Quest)
                rightController.TryGetFeatureValue(CommonUsages.primaryButton, out pressed);
            }

            return pressed;
        }

        public void SetControlScheme(ControlScheme scheme)
        {
            controlScheme = scheme;
            Debug.Log($"Control scheme changed to: {scheme}");
        }

        public ControlScheme GetControlScheme()
        {
            return controlScheme;
        }

        // Debug visualization
        private void OnGUI()
        {
            if (Debug.isDebugBuild)
            {
                GUI.Label(new Rect(10, 10, 300, 20), $"Control: {controlScheme}");
                GUI.Label(new Rect(10, 30, 300, 20), $"Input: {currentInput}");
                GUI.Label(new Rect(10, 50, 300, 20), $"Left: {leftController.isValid}");
                GUI.Label(new Rect(10, 70, 300, 20), $"Right: {rightController.isValid}");
            }
        }
    }
}
