using System.Collections.Generic;
using UnityEngine;

namespace SnakeVR
{
    public class SpecialFoodManager : MonoBehaviour
    {
        public static SpecialFoodManager Instance { get; private set; }

        [Header("Food Effects")]
        [SerializeField] private List<SpecialFoodEffect> foodEffects = new List<SpecialFoodEffect>();

        [Header("Magnet Settings")]
        [SerializeField] private float magnetRadius = 5f;
        [SerializeField] private float magnetForce = 10f;

        // Active effect timers
        private float speedBoostTimer = 0f;
        private float slowMoTimer = 0f;
        private float ghostModeTimer = 0f;
        private float magnetTimer = 0f;
        private float pointMultiplierTimer = 0f;
        private int pointMultiplierFoodsRemaining = 0;

        // Grace period for ghost mode expiration
        private float ghostGracePeriod = 0f;
        private const float GHOST_GRACE_DURATION = 0.5f;

        // References
        private SnakeController snakeController;
        private GameManager gameManager;
        private Transform headTransform;

        // Events for visual feedback
        public System.Action<FoodType, float> OnEffectStarted;
        public System.Action<FoodType> OnEffectEnded;
        public System.Action<FoodType, float> OnEffectWarning; // Called when < 2 sec remaining

        // Effect data cache
        private Dictionary<FoodType, SpecialFoodEffect> effectLookup = new Dictionary<FoodType, SpecialFoodEffect>();
        private float totalSpawnWeight = 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            BuildEffectLookup();
        }

        private void Start()
        {
            snakeController = FindObjectOfType<SnakeController>();
            gameManager = GameManager.Instance;

            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                headTransform = mainCam.transform;
            }
        }

        private void BuildEffectLookup()
        {
            effectLookup.Clear();
            totalSpawnWeight = 0f;

            foreach (var effect in foodEffects)
            {
                if (effect != null)
                {
                    effectLookup[effect.foodType] = effect;
                    totalSpawnWeight += effect.spawnWeight;
                }
            }
        }

        private void Update()
        {
            UpdateTimers();
            UpdateMagnet();
        }

        private void UpdateTimers()
        {
            // Speed Boost
            if (speedBoostTimer > 0f)
            {
                speedBoostTimer -= Time.deltaTime;
                CheckWarning(FoodType.SpeedBoost, speedBoostTimer);
                if (speedBoostTimer <= 0f)
                {
                    EndSpeedBoost();
                }
            }

            // Slow-Mo (uses unscaled time)
            if (slowMoTimer > 0f)
            {
                slowMoTimer -= Time.unscaledDeltaTime;
                CheckWarning(FoodType.SlowMo, slowMoTimer);
                if (slowMoTimer <= 0f)
                {
                    EndSlowMo();
                }
            }

            // Ghost Mode
            if (ghostModeTimer > 0f)
            {
                ghostModeTimer -= Time.deltaTime;
                CheckWarning(FoodType.GhostMode, ghostModeTimer);
                if (ghostModeTimer <= 0f)
                {
                    StartGhostGracePeriod();
                }
            }

            // Ghost grace period
            if (ghostGracePeriod > 0f)
            {
                ghostGracePeriod -= Time.deltaTime;
                if (ghostGracePeriod <= 0f)
                {
                    EndGhostMode();
                }
            }

            // Magnet
            if (magnetTimer > 0f)
            {
                magnetTimer -= Time.deltaTime;
                CheckWarning(FoodType.Magnet, magnetTimer);
                if (magnetTimer <= 0f)
                {
                    EndMagnet();
                }
            }

            // Point Multiplier (time-based backup)
            if (pointMultiplierTimer > 0f)
            {
                pointMultiplierTimer -= Time.deltaTime;
                CheckWarning(FoodType.PointMultiplier, pointMultiplierTimer);
                if (pointMultiplierTimer <= 0f)
                {
                    EndPointMultiplier();
                }
            }
        }

        private void CheckWarning(FoodType type, float timeRemaining)
        {
            if (timeRemaining > 0f && timeRemaining <= 2f)
            {
                OnEffectWarning?.Invoke(type, timeRemaining);
            }
        }

        private void UpdateMagnet()
        {
            if (magnetTimer <= 0f || headTransform == null) return;

            // Find all food in range
            Collider[] colliders = Physics.OverlapSphere(headTransform.position, magnetRadius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("Food"))
                {
                    Rigidbody rb = col.GetComponent<Rigidbody>();
                    if (rb != null && !rb.isKinematic)
                    {
                        Vector3 direction = (headTransform.position - col.transform.position).normalized;
                        rb.AddForce(direction * magnetForce, ForceMode.Acceleration);
                    }
                }
            }
        }

        public void ApplyEffect(FoodType foodType)
        {
            if (!effectLookup.TryGetValue(foodType, out SpecialFoodEffect effect))
            {
                // Normal food or unknown type
                return;
            }

            switch (foodType)
            {
                case FoodType.SpeedBoost:
                    StartSpeedBoost(effect);
                    break;
                case FoodType.SlowMo:
                    StartSlowMo(effect);
                    break;
                case FoodType.Shrink:
                    ApplyShrink(effect);
                    break;
                case FoodType.SuperGrowth:
                    ApplySuperGrowth(effect);
                    break;
                case FoodType.GhostMode:
                    StartGhostMode(effect);
                    break;
                case FoodType.PointMultiplier:
                    StartPointMultiplier(effect);
                    break;
                case FoodType.Magnet:
                    StartMagnet(effect);
                    break;
            }
        }

        // Speed Boost
        private void StartSpeedBoost(SpecialFoodEffect effect)
        {
            speedBoostTimer = effect.duration;
            if (snakeController != null)
            {
                snakeController.SetSpeedMultiplier(effect.effectMultiplier);
            }
            OnEffectStarted?.Invoke(FoodType.SpeedBoost, effect.duration);
            Debug.Log($"Speed Boost started: {effect.effectMultiplier}x for {effect.duration}s");
        }

        private void EndSpeedBoost()
        {
            if (snakeController != null)
            {
                snakeController.SetSpeedMultiplier(1f);
            }
            OnEffectEnded?.Invoke(FoodType.SpeedBoost);
            Debug.Log("Speed Boost ended");
        }

        // Slow-Mo
        private void StartSlowMo(SpecialFoodEffect effect)
        {
            slowMoTimer = effect.duration;
            if (gameManager != null)
            {
                gameManager.SetTimeScale(effect.effectMultiplier);
            }
            OnEffectStarted?.Invoke(FoodType.SlowMo, effect.duration);
            Debug.Log($"Slow-Mo started: {effect.effectMultiplier}x for {effect.duration}s");
        }

        private void EndSlowMo()
        {
            if (gameManager != null)
            {
                gameManager.SetTimeScale(1f);
            }
            OnEffectEnded?.Invoke(FoodType.SlowMo);
            Debug.Log("Slow-Mo ended");
        }

        // Shrink
        private void ApplyShrink(SpecialFoodEffect effect)
        {
            if (snakeController != null)
            {
                snakeController.RemoveSegments(Mathf.Abs(effect.segmentChange));
            }
            Debug.Log($"Shrink applied: removed {Mathf.Abs(effect.segmentChange)} segments");
        }

        // Super Growth
        private void ApplySuperGrowth(SpecialFoodEffect effect)
        {
            if (snakeController != null)
            {
                snakeController.AddSegments(effect.segmentChange);
            }
            Debug.Log($"Super Growth applied: added {effect.segmentChange} segments");
        }

        // Ghost Mode
        private void StartGhostMode(SpecialFoodEffect effect)
        {
            ghostModeTimer = effect.duration;
            ghostGracePeriod = 0f; // Cancel any pending grace period
            if (snakeController != null)
            {
                snakeController.SetGhostMode(true);
            }
            OnEffectStarted?.Invoke(FoodType.GhostMode, effect.duration);
            Debug.Log($"Ghost Mode started for {effect.duration}s");
        }

        private void StartGhostGracePeriod()
        {
            ghostGracePeriod = GHOST_GRACE_DURATION;
            Debug.Log("Ghost Mode grace period started");
        }

        private void EndGhostMode()
        {
            if (snakeController != null)
            {
                snakeController.SetGhostMode(false);
            }
            OnEffectEnded?.Invoke(FoodType.GhostMode);
            Debug.Log("Ghost Mode ended");
        }

        // Point Multiplier
        private void StartPointMultiplier(SpecialFoodEffect effect)
        {
            pointMultiplierTimer = 15f; // Max 15 seconds
            pointMultiplierFoodsRemaining = effect.foodsAffected;
            if (gameManager != null)
            {
                gameManager.SetPointMultiplier(effect.effectMultiplier);
            }
            OnEffectStarted?.Invoke(FoodType.PointMultiplier, pointMultiplierTimer);
            Debug.Log($"Point Multiplier started: {effect.effectMultiplier}x for {effect.foodsAffected} foods");
        }

        public void OnFoodEatenForMultiplier()
        {
            if (pointMultiplierFoodsRemaining > 0)
            {
                pointMultiplierFoodsRemaining--;
                if (pointMultiplierFoodsRemaining <= 0)
                {
                    EndPointMultiplier();
                }
            }
        }

        private void EndPointMultiplier()
        {
            pointMultiplierTimer = 0f;
            pointMultiplierFoodsRemaining = 0;
            if (gameManager != null)
            {
                gameManager.SetPointMultiplier(1f);
            }
            OnEffectEnded?.Invoke(FoodType.PointMultiplier);
            Debug.Log("Point Multiplier ended");
        }

        // Magnet
        private void StartMagnet(SpecialFoodEffect effect)
        {
            magnetTimer = effect.duration;
            OnEffectStarted?.Invoke(FoodType.Magnet, effect.duration);
            Debug.Log($"Magnet started for {effect.duration}s");
        }

        private void EndMagnet()
        {
            OnEffectEnded?.Invoke(FoodType.Magnet);
            Debug.Log("Magnet ended");
        }

        // Food type selection for spawning
        public FoodType GetRandomFoodType()
        {
            if (foodEffects.Count == 0 || totalSpawnWeight <= 0f)
            {
                return FoodType.Normal;
            }

            float random = Random.Range(0f, totalSpawnWeight);
            float cumulative = 0f;

            foreach (var effect in foodEffects)
            {
                if (effect == null) continue;
                cumulative += effect.spawnWeight;
                if (random <= cumulative)
                {
                    return effect.foodType;
                }
            }

            return FoodType.Normal;
        }

        public SpecialFoodEffect GetEffect(FoodType foodType)
        {
            effectLookup.TryGetValue(foodType, out SpecialFoodEffect effect);
            return effect;
        }

        public Color GetFoodColor(FoodType foodType)
        {
            if (effectLookup.TryGetValue(foodType, out SpecialFoodEffect effect))
            {
                return effect.color;
            }
            return Color.green; // Default normal food color
        }

        public float GetPointMultiplier(FoodType foodType)
        {
            if (effectLookup.TryGetValue(foodType, out SpecialFoodEffect effect))
            {
                return effect.pointMultiplier;
            }
            return 1f;
        }

        // Clear all effects (on game over)
        public void ClearAllEffects()
        {
            if (speedBoostTimer > 0f) EndSpeedBoost();
            if (slowMoTimer > 0f) EndSlowMo();
            if (ghostModeTimer > 0f || ghostGracePeriod > 0f)
            {
                ghostModeTimer = 0f;
                ghostGracePeriod = 0f;
                EndGhostMode();
            }
            if (magnetTimer > 0f) EndMagnet();
            if (pointMultiplierTimer > 0f || pointMultiplierFoodsRemaining > 0) EndPointMultiplier();
        }

        // Check if specific effect is active
        public bool IsEffectActive(FoodType foodType)
        {
            return foodType switch
            {
                FoodType.SpeedBoost => speedBoostTimer > 0f,
                FoodType.SlowMo => slowMoTimer > 0f,
                FoodType.GhostMode => ghostModeTimer > 0f || ghostGracePeriod > 0f,
                FoodType.Magnet => magnetTimer > 0f,
                FoodType.PointMultiplier => pointMultiplierTimer > 0f || pointMultiplierFoodsRemaining > 0,
                _ => false
            };
        }

        // Get active effect color (for controller glow)
        public Color GetActiveEffectColor()
        {
            // Priority: Ghost > SlowMo > SpeedBoost > Magnet > PointMultiplier
            if (ghostModeTimer > 0f || ghostGracePeriod > 0f)
                return GetFoodColor(FoodType.GhostMode);
            if (slowMoTimer > 0f)
                return GetFoodColor(FoodType.SlowMo);
            if (speedBoostTimer > 0f)
                return GetFoodColor(FoodType.SpeedBoost);
            if (magnetTimer > 0f)
                return GetFoodColor(FoodType.Magnet);
            if (pointMultiplierTimer > 0f || pointMultiplierFoodsRemaining > 0)
                return GetFoodColor(FoodType.PointMultiplier);

            return Color.clear;
        }

        public bool HasActiveEffect()
        {
            return speedBoostTimer > 0f || slowMoTimer > 0f ||
                   ghostModeTimer > 0f || ghostGracePeriod > 0f ||
                   magnetTimer > 0f || pointMultiplierTimer > 0f ||
                   pointMultiplierFoodsRemaining > 0;
        }
    }
}
