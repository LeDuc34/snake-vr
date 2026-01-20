using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace SnakeVR.UI
{
    /// <summary>
    /// Arcade-style menu button with hover effects and pointer interaction.
    /// </summary>
    public class ArcadeMenuButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI buttonText;
        [SerializeField] private Collider buttonCollider;

        [Header("Colors")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = new Color(1f, 0.84f, 0f); // Yellow

        [Header("Hover Effects")]
        [SerializeField] private float normalScale = 1f;
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float scaleSpeed = 10f;
        [SerializeField] private float glowIntensity = 1.5f;

        [Header("Events")]
        public UnityEvent OnClick;

        private bool isHovered = false;
        private float currentScale = 1f;
        private Vector3 originalScale;
        private Material textMaterial;

        private void Awake()
        {
            if (buttonText == null)
            {
                buttonText = GetComponentInChildren<TextMeshProUGUI>();
            }

            if (buttonCollider == null)
            {
                buttonCollider = GetComponent<Collider>();
                if (buttonCollider == null)
                {
                    // Add a box collider for ray interaction
                    BoxCollider box = gameObject.AddComponent<BoxCollider>();
                    box.size = new Vector3(2f, 0.5f, 0.1f);
                    buttonCollider = box;
                }
            }

            originalScale = transform.localScale;

            if (buttonText != null)
            {
                buttonText.color = normalColor;
            }
        }

        private void Update()
        {
            // Smoothly scale towards target
            float targetScale = isHovered ? hoverScale : normalScale;
            currentScale = Mathf.Lerp(currentScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
            transform.localScale = originalScale * currentScale;
        }

        public void SetHovered(bool hovered)
        {
            if (isHovered == hovered) return;

            isHovered = hovered;

            if (buttonText != null)
            {
                buttonText.color = hovered ? hoverColor : normalColor;

                // Increase font glow when hovered
                if (buttonText.fontMaterial != null)
                {
                    buttonText.fontMaterial.SetFloat("_GlowPower", hovered ? glowIntensity : 0.5f);
                }
            }
        }

        public void Click()
        {
            // Flash effect
            StartCoroutine(FlashEffect());
            OnClick?.Invoke();
        }

        private System.Collections.IEnumerator FlashEffect()
        {
            if (buttonText != null)
            {
                Color original = buttonText.color;
                buttonText.color = Color.white;
                yield return new WaitForSecondsRealtime(0.1f);
                buttonText.color = isHovered ? hoverColor : normalColor;
            }
        }

        public void SetText(string text)
        {
            if (buttonText != null)
            {
                buttonText.text = text;
            }
        }

        public void SetColor(Color color)
        {
            normalColor = color;
            if (!isHovered && buttonText != null)
            {
                buttonText.color = color;
            }
        }
    }
}
