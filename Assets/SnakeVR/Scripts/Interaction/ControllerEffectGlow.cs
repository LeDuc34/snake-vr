using UnityEngine;

namespace SnakeVR
{
    public class ControllerEffectGlow : MonoBehaviour
    {
        [Header("Glow Settings")]
        [SerializeField] private float glowIntensity = 2f;
        [SerializeField] private float pulseSpeed = 4f;

        private GameObject glowObject;
        private Renderer glowRenderer;
        private Material glowMaterial;
        private Color currentColor = Color.clear;
        private bool isPulsing = false;
        private float pulseTime = 0f;

        private void Start()
        {
            CreateGlowObject();

            // Subscribe to effect events
            if (SpecialFoodManager.Instance != null)
            {
                SpecialFoodManager.Instance.OnEffectStarted += OnEffectStarted;
                SpecialFoodManager.Instance.OnEffectEnded += OnEffectEnded;
                SpecialFoodManager.Instance.OnEffectWarning += OnEffectWarning;
            }
        }

        private void CreateGlowObject()
        {
            // Create a small sphere as the glow indicator
            glowObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            glowObject.name = "EffectGlow";
            glowObject.transform.SetParent(transform);
            glowObject.transform.localPosition = Vector3.zero;
            glowObject.transform.localScale = Vector3.one * 0.08f;

            // Remove collider
            Collider col = glowObject.GetComponent<Collider>();
            if (col != null)
            {
                Destroy(col);
            }

            // Set up material
            glowRenderer = glowObject.GetComponent<Renderer>();
            glowMaterial = new Material(Shader.Find("Standard"));
            glowMaterial.SetFloat("_Metallic", 0f);
            glowMaterial.SetFloat("_Glossiness", 1f);
            glowMaterial.EnableKeyword("_EMISSION");
            glowRenderer.material = glowMaterial;

            // Start hidden
            glowObject.SetActive(false);
        }

        private void Update()
        {
            if (SpecialFoodManager.Instance == null) return;

            // Update glow based on active effects
            if (SpecialFoodManager.Instance.HasActiveEffect())
            {
                Color effectColor = SpecialFoodManager.Instance.GetActiveEffectColor();
                if (effectColor != currentColor)
                {
                    currentColor = effectColor;
                    UpdateGlowColor();
                }

                if (!glowObject.activeSelf)
                {
                    glowObject.SetActive(true);
                }

                // Handle pulsing
                if (isPulsing)
                {
                    pulseTime += Time.unscaledDeltaTime * pulseSpeed;
                    float pulse = (Mathf.Sin(pulseTime) + 1f) * 0.5f; // 0 to 1
                    float intensity = Mathf.Lerp(0.5f, glowIntensity, pulse);
                    glowMaterial.SetColor("_EmissionColor", currentColor * intensity);
                }
            }
            else
            {
                if (glowObject.activeSelf)
                {
                    glowObject.SetActive(false);
                    isPulsing = false;
                }
            }
        }

        private void UpdateGlowColor()
        {
            if (glowMaterial != null)
            {
                glowMaterial.color = currentColor;
                glowMaterial.SetColor("_EmissionColor", currentColor * glowIntensity);
            }
        }

        private void OnEffectStarted(FoodType foodType, float duration)
        {
            isPulsing = false;
            pulseTime = 0f;
        }

        private void OnEffectEnded(FoodType foodType)
        {
            // Check if any effects still active
            if (SpecialFoodManager.Instance != null && !SpecialFoodManager.Instance.HasActiveEffect())
            {
                isPulsing = false;
                currentColor = Color.clear;
            }
        }

        private void OnEffectWarning(FoodType foodType, float timeRemaining)
        {
            // Start pulsing when effect is about to expire
            isPulsing = true;
        }

        private void OnDestroy()
        {
            // Unsubscribe from events
            if (SpecialFoodManager.Instance != null)
            {
                SpecialFoodManager.Instance.OnEffectStarted -= OnEffectStarted;
                SpecialFoodManager.Instance.OnEffectEnded -= OnEffectEnded;
                SpecialFoodManager.Instance.OnEffectWarning -= OnEffectWarning;
            }

            // Clean up
            if (glowMaterial != null)
            {
                Destroy(glowMaterial);
            }
            if (glowObject != null)
            {
                Destroy(glowObject);
            }
        }
    }
}
