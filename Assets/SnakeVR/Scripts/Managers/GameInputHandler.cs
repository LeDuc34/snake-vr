using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeVR
{
    /// <summary>
    /// Handles game input (pause, start) using native XRI Input Actions.
    /// Attach this to any persistent GameObject in the scene.
    /// </summary>
    public class GameInputHandler : MonoBehaviour
    {
        [Header("Input Actions - Native XRI")]
        [Tooltip("Reference to XRI Left/Menu action for pause")]
        [SerializeField] private InputActionReference menuAction;

        [Tooltip("Reference to XRI Right/Primary Button action for start")]
        [SerializeField] private InputActionReference primaryButtonAction;

        private bool menuWasPressed = false;
        private bool startWasPressed = false;

        private void OnEnable()
        {
            if (menuAction != null && menuAction.action != null)
            {
                menuAction.action.Enable();
            }
            if (primaryButtonAction != null && primaryButtonAction.action != null)
            {
                primaryButtonAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (menuAction != null && menuAction.action != null)
            {
                menuAction.action.Disable();
            }
            if (primaryButtonAction != null && primaryButtonAction.action != null)
            {
                primaryButtonAction.action.Disable();
            }
        }

        private void Update()
        {
            HandlePauseInput();
            HandleStartInput();
        }

        private void HandlePauseInput()
        {
            bool menuPressed = IsMenuPressed();

            if (menuPressed && !menuWasPressed)
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.PauseGame();
                }
            }
            menuWasPressed = menuPressed;
        }

        private void HandleStartInput()
        {
            bool startPressed = IsPrimaryButtonPressed();

            if (startPressed && !startWasPressed)
            {
                if (GameManager.Instance != null &&
                    GameManager.Instance.GetCurrentState() == GameState.Menu)
                {
                    GameManager.Instance.StartGame();
                }
            }
            startWasPressed = startPressed;
        }

        private bool IsMenuPressed()
        {
            if (menuAction != null && menuAction.action != null)
            {
                return menuAction.action.IsPressed();
            }

            // Keyboard fallback
            if (Keyboard.current != null && Keyboard.current.pKey.isPressed)
            {
                return true;
            }

            return false;
        }

        private bool IsPrimaryButtonPressed()
        {
            if (primaryButtonAction != null && primaryButtonAction.action != null)
            {
                return primaryButtonAction.action.IsPressed();
            }

            // Keyboard fallback
            if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
            {
                return true;
            }

            return false;
        }
    }
}
