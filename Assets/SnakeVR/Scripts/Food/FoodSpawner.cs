using UnityEngine;

namespace SnakeVR
{
    public class FoodSpawner : MonoBehaviour
    {
        [Header("Food Settings")]
        [SerializeField] private GameObject foodPrefab;
        [SerializeField] private float foodSize = 0.2f;
        [SerializeField] private Color normalFoodColor = new Color(0.298f, 0.686f, 0.314f); // Green

        [Header("Special Food")]
        [SerializeField] private float specialFoodChance = 0.18f; // 18% chance for special food

        [Header("Spawn Area")]
        [SerializeField] private Vector3 spawnAreaSize = new Vector3(5f, 3f, 5f);
        [SerializeField] private float gridStep = 0.3f;
        [SerializeField] private bool visualizeSpawnArea = true;

        private GameObject currentFood;
        private SnakeController snakeController;

        private void Start()
        {
            snakeController = FindObjectOfType<SnakeController>();

            // Create default food prefab if none assigned
            if (foodPrefab == null)
            {
                CreateDefaultFoodPrefab();
            }
        }

        public void SpawnFood()
        {
            // Destroy existing food
            if (currentFood != null)
            {
                Destroy(currentFood);
            }

            // Find valid spawn position
            Vector3 spawnPosition = GetRandomSpawnPosition();
            int maxAttempts = 50;
            int attempts = 0;

            while (IsPositionOccupied(spawnPosition) && attempts < maxAttempts)
            {
                spawnPosition = GetRandomSpawnPosition();
                attempts++;
            }

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Could not find valid spawn position for food!");
                return;
            }

            // Spawn food
            currentFood = Instantiate(foodPrefab, spawnPosition, Quaternion.identity);
            currentFood.tag = "Food";

            // Determine food type
            FoodType foodType = DetermineSpawnType();

            // Add GrabbableFood component
            GrabbableFood grabbable = currentFood.GetComponent<GrabbableFood>();
            if (grabbable == null)
            {
                grabbable = currentFood.AddComponent<GrabbableFood>();
            }
            grabbable.SetFoodType(foodType);

            // Add FoodVisual component and set color
            FoodVisual visual = currentFood.GetComponent<FoodVisual>();
            if (visual == null)
            {
                visual = currentFood.AddComponent<FoodVisual>();
            }
            visual.SetFoodType(foodType);

            // Add Rigidbody for physics
            Rigidbody rb = currentFood.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = currentFood.AddComponent<Rigidbody>();
            }
            rb.useGravity = false;
            rb.linearDamping = 2f; // Slow down thrown food
            rb.angularDamping = 2f;

            Debug.Log($"Food spawned at {spawnPosition}: {foodType}");
        }

        private FoodType DetermineSpawnType()
        {
            // Check if special food should spawn
            if (Random.value > specialFoodChance)
            {
                return FoodType.Normal;
            }

            // Get random special food type from manager
            if (SpecialFoodManager.Instance != null)
            {
                return SpecialFoodManager.Instance.GetRandomFoodType();
            }

            // Fallback: random special type if no manager
            return GetRandomSpecialType();
        }

        private FoodType GetRandomSpecialType()
        {
            // Weighted random selection (fallback if no SpecialFoodManager)
            float roll = Random.value;

            // Common (25% each within special pool)
            if (roll < 0.25f) return FoodType.SpeedBoost;
            if (roll < 0.50f) return FoodType.SlowMo;
            if (roll < 0.75f) return FoodType.SuperGrowth;

            // Uncommon (15% each)
            if (roll < 0.85f) return FoodType.Shrink;
            if (roll < 0.95f) return FoodType.PointMultiplier;

            // Rare (5% each)
            if (roll < 0.975f) return FoodType.GhostMode;
            return FoodType.Magnet;
        }

        private Vector3 GetRandomSpawnPosition()
        {
            // Snap to grid for cleaner gameplay
            float x = Mathf.Round(Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2) / gridStep) * gridStep;
            float y = Mathf.Round(Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2) / gridStep) * gridStep;
            float z = Mathf.Round(Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2) / gridStep) * gridStep;

            return transform.position + new Vector3(x, y, z);
        }

        private bool IsPositionOccupied(Vector3 position)
        {
            if (snakeController == null)
                return false;

            // Check if position is too close to snake
            var occupiedPositions = snakeController.GetOccupiedPositions();
            foreach (var pos in occupiedPositions)
            {
                if (Vector3.Distance(position, pos) < gridStep)
                {
                    return true;
                }
            }

            return false;
        }

        private void CreateDefaultFoodPrefab()
        {
            GameObject food = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            food.transform.localScale = Vector3.one * foodSize;

            // Set up collider for physics bouncing
            Collider col = food.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = false; // Changed from true - needed for physics bouncing
            }

            // Set default color (will be overwritten by FoodVisual)
            Renderer renderer = food.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = normalFoodColor;
                mat.SetFloat("_Metallic", 0.5f);
                mat.SetFloat("_Glossiness", 0.8f);
                renderer.material = mat;
            }

            // Make it a prefab in memory
            foodPrefab = food;
            food.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            if (visualizeSpawnArea)
            {
                Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
                Gizmos.DrawWireCube(transform.position, spawnAreaSize);
            }
        }

        public void ClearFood()
        {
            if (currentFood != null)
            {
                Destroy(currentFood);
                currentFood = null;
            }
        }

        public Vector3 GetSpawnAreaSize()
        {
            return spawnAreaSize;
        }
    }
}
