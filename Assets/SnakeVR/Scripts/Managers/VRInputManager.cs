using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeVR
{
    public enum ControlScheme
    {
        LeftJoystick,
        RightJoystick,
        HeadGaze,
        RightControllerDirection
    }

    /// <summary>
    /// Action-based VR Input Manager compatible with XR Interaction Toolkit 3.x
    /// Uses InputActionAsset for proper binding resolution with XR Simulator and real hardware.
    /// Assign "XRI Default Input Actions" in the inspector.
    /// </summary>
    public class VRInputManager : MonoBehaviour
    {
        [Header("Control Settings")]
        [SerializeField] private ControlScheme controlScheme = ControlScheme.LeftJoystick;
        [SerializeField] private float joystickDeadzone = 0.3f;

        [Header("Input Action Asset")]
        [Tooltip("Assign 'XRI Default Input Actions' from Samples/XR Interaction Toolkit/Starter Assets")]
        [SerializeField] private InputActionAsset inputActionAsset;

        [Header("Head Gaze Settings")]
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private float gazeSmoothing = 5f;

        // Resolved actions from the asset
        private InputAction leftThumbstickAction;
        private InputAction rightThumbstickAction;
        private InputAction leftMenuAction;
        private InputAction rightPrimaryButtonAction;

        // Cached input
        private Vector2 currentInput = Vector2.zero;

        // Button state tracking for "pressed this frame" detection
        private bool menuWasPressed = false;
        private bool startWasPressed = false;

        private void Awake()
        {
            ResolveActions();
        }

        private void OnEnable()
        {
            if (inputActionAsset != null)
            {
                inputActionAsset.Enable();
            }
        }

        private void OnDisable()
        {
            if (inputActionAsset != null)
            {
                inputActionAsset.Disable();
            }
        }

        private void ResolveActions()
        {
            if (inputActionAsset == null)
            {
                Debug.LogError("[VRInputManager] InputActionAsset not assigned! Assign 'XRI Default Input Actions' in the inspector.");
                return;
            }

            // Find actions from the XRI Default Input Actions asset
            // Action Maps are "XRI Left" and "XRI Right"
            leftThumbstickAction = inputActionAsset.FindAction("XRI Left/Thumbstick");
            rightThumbstickAction = inputActionAsset.FindAction("XRI Right/Thumbstick");
            leftMenuAction = inputActionAsset.FindAction("XRI Left/Menu");
            rightPrimaryButtonAction = inputActionAsset.FindAction("XRI Right/Primary Button");

            if (leftThumbstickAction == null)
                Debug.LogWarning("[VRInputManager] Could not find 'XRI LeftHand Interaction/Thumbstick' action");
            if (rightThumbstickAction == null)
                Debug.LogWarning("[VRInputManager] Could not find 'XRI RightHand Interaction/Thumbstick' action");
        }

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
        }

        private void Update()
        {
            // Update input based on control scheme
            UpdateInput();

            // Check for menu button (pressed this frame)
            bool menuPressed = GetMenuButton();
            if (menuPressed && !menuWasPressed)
            {
                Debug.Log("[VRInputManager] Menu/Pause button pressed!");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PauseGame();
                }
            }
            menuWasPressed = menuPressed;

            // Check for start button (pressed this frame)
            bool startPressed = GetStartButton();
            if (startPressed && !startWasPressed)
            {
                if (GameManager.Instance != null && GameManager.Instance.GetCurrentState() == GameState.Menu)
                {
                    GameManager.Instance.StartGame();
                }
            }
            startWasPressed = startPressed;
        }

        private void UpdateInput()
        {
            switch (controlScheme)
            {
                case ControlScheme.LeftJoystick:
                    currentInput = GetThumbstickInput(leftThumbstickAction);
                    // Fallback to right if left has no input
                    if (currentInput == Vector2.zero)
                    {
                        currentInput = GetThumbstickInput(rightThumbstickAction);
                    }
                    break;

                case ControlScheme.RightJoystick:
                    currentInput = GetThumbstickInput(rightThumbstickAction);
                    // Fallback to left if right has no input
                    if (currentInput == Vector2.zero)
                    {
                        currentInput = GetThumbstickInput(leftThumbstickAction);
                    }
                    break;

                case ControlScheme.HeadGaze:
                    currentInput = GetHeadGazeInput();
                    break;

                case ControlScheme.RightControllerDirection:
                    currentInput = GetControllerDirectionInput();
                    break;
            }
        }

        private Vector2 GetThumbstickInput(InputAction action)
        {
            if (action == null)
                return Vector2.zero;

            Vector2 value = action.ReadValue<Vector2>();

            // Apply deadzone
            if (value.magnitude < joystickDeadzone)
            {
                return Vector2.zero;
            }

            return value;
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

        private Vector2 GetControllerDirectionInput()
        {
            // This would need a rotation action from the asset
            // For now, return zero - can be implemented if needed
            return Vector2.zero;
        }

        public Vector2 GetMovementInput()
        {
            return currentInput;
        }

        private bool GetMenuButton()
        {
            // Check left menu button
            if (leftMenuAction != null && leftMenuAction.IsPressed())
            {
                return true;
            }

            // Keyboard fallback: P key
            if (Keyboard.current != null && Keyboard.current.pKey.isPressed)
            {
                return true;
            }

            return false;
        }

        private bool GetStartButton()
        {
            // Check right primary button (A on Quest)
            if (rightPrimaryButtonAction != null && rightPrimaryButtonAction.IsPressed())
            {
                return true;
            }

            // Keyboard fallback: Space key
            if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
            {
                return true;
            }

            return false;
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
                bool leftConnected = leftThumbstickAction?.activeControl != null;
                bool rightConnected = rightThumbstickAction?.activeControl != null;

                GUI.Label(new Rect(10, 10, 300, 20), $"Control: {controlScheme}");
                GUI.Label(new Rect(10, 30, 300, 20), $"Input: {currentInput}");
                GUI.Label(new Rect(10, 50, 300, 20), $"Left Controller: {(leftConnected ? "Connected" : "Not found")}");
                GUI.Label(new Rect(10, 70, 300, 20), $"Right Controller: {(rightConnected ? "Connected" : "Not found")}");
            }
        }
    }
}
