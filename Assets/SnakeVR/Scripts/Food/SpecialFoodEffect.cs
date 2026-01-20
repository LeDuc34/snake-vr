using UnityEngine;

namespace SnakeVR
{
    [CreateAssetMenu(fileName = "NewFoodEffect", menuName = "SnakeVR/Food Effect")]
    public class SpecialFoodEffect : ScriptableObject
    {
        [Header("Identity")]
        public FoodType foodType;
        public Color color = Color.white;

        [Header("Spawning")]
        [Tooltip("Higher weight = more likely to spawn")]
        public float spawnWeight = 1f;

        [Header("Effect Settings")]
        [Tooltip("Duration in seconds (0 for instant effects)")]
        public float duration = 0f;

        [Tooltip("For speed/multiplier effects")]
        public float effectMultiplier = 1f;

        [Tooltip("For growth/shrink effects")]
        public int segmentChange = 0;

        [Tooltip("For point multiplier: number of foods affected")]
        public int foodsAffected = 0;

        [Tooltip("Base points awarded (multiplied by GameManager.scorePerFood)")]
        public float pointMultiplier = 1f;
    }
}
