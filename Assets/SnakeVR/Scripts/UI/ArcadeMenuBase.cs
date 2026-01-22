using System.Collections.Generic;
using UnityEngine;

namespace SnakeVR.UI
{
    /// <summary>
    /// Base class for arcade-style menus.
    /// Button interaction is now handled by XR Interaction Toolkit (XRSimpleInteractable on buttons).
    /// </summary>
    public abstract class ArcadeMenuBase : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] protected AudioClip hoverSound;
        [SerializeField] protected AudioClip selectSound;
        [SerializeField] protected AudioSource audioSource;

        [Header("Visual")]
        [SerializeField] protected CanvasGroup canvasGroup;
        [SerializeField] protected float fadeSpeed = 5f;

        protected List<ArcadeMenuButton> buttons = new List<ArcadeMenuButton>();

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

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
        }

        protected virtual void Start()
        {
        }

        protected virtual void Update()
        {
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
