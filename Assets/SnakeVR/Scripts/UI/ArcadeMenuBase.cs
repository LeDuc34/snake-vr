using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.InputSystem;
using XRInputDevice = UnityEngine.XR.InputDevice;
using XRCommonUsages = UnityEngine.XR.CommonUsages;

namespace SnakeVR.UI
{
    /// <summary>
    /// Base class for arcade-style menus with controller pointer interaction.
    /// Handles raycasting from controllers and button hover/click states.
    /// </summary>
    public abstract class ArcadeMenuBase : MonoBehaviour
    {
        [Header("Pointer Settings")]
        [SerializeField] protected Transform leftHandTransform;
        [SerializeField] protected Transform rightHandTransform;
        [SerializeField] protected LineRenderer pointerLine;
        [SerializeField] protected float pointerLength = 10f;
        [SerializeField] protected Color pointerColor = new Color(0f, 1f, 1f); // Cyan

        [Header("Audio")]
        [SerializeField] protected AudioClip hoverSound;
        [SerializeField] protected AudioClip selectSound;
        [SerializeField] protected AudioSource audioSource;

        [Header("Visual")]
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected float fadeSpeed = 5f;

        protected List<ArcadeMenuButton> buttons = new List<ArcadeMenuButton>();
        protected ArcadeMenuButton currentHoveredButton;

        // Input
        private XRInputDevice leftController;
        private XRInputDevice rightController;
        private bool triggerWasPressed = false;

        protected virtual void Awake()
        {
            // Find buttons in children
            buttons.AddRange(GetComponentsInChildren<ArcadeMenuButton>(true));

            // Create audio source if not assigned
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f; // 2D sound
            }

            // Create pointer line if not assigned
            if (pointerLine == null)
            {
                GameObject lineObj = new GameObject("PointerLine");
                lineObj.transform.SetParent(transform);
                pointerLine = lineObj.AddComponent<LineRenderer>();
                pointerLine.startWidth = 0.005f;
                pointerLine.endWidth = 0.005f;
                pointerLine.material = new Material(Shader.Find("Sprites/Default"));
                pointerLine.startColor = pointerColor;
                pointerLine.endColor = pointerColor;
                pointerLine.positionCount = 2;
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        protected virtual void Start()
        {
            // Get controllers
            leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

            // Try to find hand transforms if not assigned
            if (leftHandTransform == null || rightHandTransform == null)
            {
                FindHandTransforms();
            }
        }

        protected virtual void Update()
        {
            // Refresh devices if not valid
            if (!leftController.isValid)
            {
                leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
            }
            if (!rightController.isValid)
            {
                rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
            }

            // Update pointer and handle input
            UpdatePointer();
            HandleTriggerInput();
        }

        private void FindHandTransforms()
        {
            // Try to find XR controller transforms
            var controllers = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.XRBaseController>();
            foreach (var controller in controllers)
            {
                if (controller.name.ToLower().Contains("left"))
                {
                    leftHandTransform = controller.transform;
                }
                else if (controller.name.ToLower().Contains("right"))
                {
                    rightHandTransform = controller.transform;
                }
            }
        }

        private void UpdatePointer()
        {
            Transform activeHand = GetActiveHand();

            if (activeHand == null)
            {
                // Fallback: use mouse in editor
                #if UNITY_EDITOR
                UpdateMousePointer();
                #else
                pointerLine.enabled = false;
                #endif
                return;
            }

            pointerLine.enabled = true;

            Vector3 origin = activeHand.position;
            Vector3 direction = activeHand.forward;
            Vector3 endPoint = origin + direction * pointerLength;

            // Raycast to check for buttons
            RaycastHit hit;
            ArcadeMenuButton hitButton = null;

            if (Physics.Raycast(origin, direction, out hit, pointerLength))
            {
                endPoint = hit.point;
                hitButton = hit.collider.GetComponent<ArcadeMenuButton>();
                if (hitButton == null)
                {
                    hitButton = hit.collider.GetComponentInParent<ArcadeMenuButton>();
                }
            }

            // Update pointer line
            pointerLine.SetPosition(0, origin);
            pointerLine.SetPosition(1, endPoint);

            // Update hover state
            UpdateHoverState(hitButton);
        }

        #if UNITY_EDITOR
        private void UpdateMousePointer()
        {
            pointerLine.enabled = false;

            Camera cam = Camera.main;
            if (cam == null) return;

            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            ArcadeMenuButton hitButton = null;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                hitButton = hit.collider.GetComponent<ArcadeMenuButton>();
                if (hitButton == null)
                {
                    hitButton = hit.collider.GetComponentInParent<ArcadeMenuButton>();
                }
            }

            UpdateHoverState(hitButton);

            // Mouse click
            if (Mouse.current.leftButton.wasPressedThisFrame && currentHoveredButton != null)
            {
                PlaySelectSound();
                currentHoveredButton.Click();
            }
        }
        #endif

        private Transform GetActiveHand()
        {
            // Prefer right hand if available
            if (rightHandTransform != null && rightController.isValid)
            {
                return rightHandTransform;
            }
            if (leftHandTransform != null && leftController.isValid)
            {
                return leftHandTransform;
            }
            return rightHandTransform ?? leftHandTransform;
        }

        private void UpdateHoverState(ArcadeMenuButton newButton)
        {
            if (newButton != currentHoveredButton)
            {
                // Unhover old button
                if (currentHoveredButton != null)
                {
                    currentHoveredButton.SetHovered(false);
                }

                // Hover new button
                currentHoveredButton = newButton;
                if (currentHoveredButton != null)
                {
                    currentHoveredButton.SetHovered(true);
                    PlayHoverSound();
                }
            }
        }

        private void HandleTriggerInput()
        {
            bool triggerPressed = GetTriggerPressed();

            // Detect trigger press (not hold)
            if (triggerPressed && !triggerWasPressed)
            {
                if (currentHoveredButton != null)
                {
                    PlaySelectSound();
                    currentHoveredButton.Click();
                }
            }

            triggerWasPressed = triggerPressed;
        }

        private bool GetTriggerPressed()
        {
            bool leftTrigger = false, rightTrigger = false;

            if (leftController.isValid)
            {
                leftController.TryGetFeatureValue(XRCommonUsages.triggerButton, out leftTrigger);
            }
            if (rightController.isValid)
            {
                rightController.TryGetFeatureValue(XRCommonUsages.triggerButton, out rightTrigger);
            }

            return leftTrigger || rightTrigger;
        }

        protected void PlayHoverSound()
        {
            if (audioSource != null && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }

        protected void PlaySelectSound()
        {
            if (audioSource != null && selectSound != null)
            {
                audioSource.PlayOneShot(selectSound);
            }
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            if (canvasGroup != null)
            {
                StartCoroutine(FadeIn());
            }
        }

        public virtual void Hide()
        {
            if (canvasGroup != null)
            {
                StartCoroutine(FadeOut());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private System.Collections.IEnumerator FadeIn()
        {
            canvasGroup.alpha = 0f;
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime * fadeSpeed;
                yield return null;
            }
            canvasGroup.alpha = 1f;
        }

        private System.Collections.IEnumerator FadeOut()
        {
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha -= Time.unscaledDeltaTime * fadeSpeed;
                yield return null;
            }
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }
    }
}
