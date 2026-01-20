using UnityEngine;

namespace SnakeVR
{
    public class FoodVisual : MonoBehaviour
    {
        private Renderer foodRenderer;
        private Material foodMaterial;

        private void Awake()
        {
            foodRenderer = GetComponent<Renderer>();
        }

        public void SetColor(Color color)
        {
            if (foodRenderer == null)
            {
                foodRenderer = GetComponent<Renderer>();
            }

            if (foodRenderer != null)
            {
                // Create instance material to avoid shared material issues
                if (foodMaterial == null)
                {
                    foodMaterial = new Material(Shader.Find("Standard"));
                    foodMaterial.SetFloat("_Metallic", 0.5f);
                    foodMaterial.SetFloat("_Glossiness", 0.8f);
                    foodRenderer.material = foodMaterial;
                }

                foodMaterial.color = color;

                // Add emission for special foods (not green/normal)
                if (color != new Color(0.298f, 0.686f, 0.314f, 1f)) // Not green
                {
                    foodMaterial.EnableKeyword("_EMISSION");
                    foodMaterial.SetColor("_EmissionColor", color * 0.3f);
                }
            }
        }

        public void SetFoodType(FoodType foodType)
        {
            if (SpecialFoodManager.Instance != null)
            {
                Color color = SpecialFoodManager.Instance.GetFoodColor(foodType);
                SetColor(color);
            }
            else
            {
                // Fallback colors if manager not available
                SetColor(GetDefaultColor(foodType));
            }
        }

        private Color GetDefaultColor(FoodType foodType)
        {
            return foodType switch
            {
                FoodType.Normal => new Color(0.298f, 0.686f, 0.314f), // #4CAF50
                FoodType.SpeedBoost => new Color(1f, 0.596f, 0f),     // #FF9800
                FoodType.SlowMo => new Color(0.129f, 0.588f, 0.953f), // #2196F3
                FoodType.Shrink => new Color(0.914f, 0.118f, 0.388f), // #E91E63
                FoodType.SuperGrowth => new Color(1f, 0.922f, 0.231f),// #FFEB3B
                FoodType.GhostMode => new Color(0f, 0.737f, 0.831f),  // #00BCD4
                FoodType.PointMultiplier => new Color(1f, 0.843f, 0f),// #FFD700
                FoodType.Magnet => new Color(0.612f, 0.153f, 0.69f),  // #9C27B0
                _ => Color.green
            };
        }

        private void OnDestroy()
        {
            // Clean up material instance
            if (foodMaterial != null)
            {
                Destroy(foodMaterial);
            }
        }
    }
}
