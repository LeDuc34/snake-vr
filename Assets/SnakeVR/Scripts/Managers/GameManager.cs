using UnityEngine;
using UnityEngine.SceneManagement;

namespace SnakeVR
{
    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private float initialSpeed = 2f;
        [SerializeField] private float speedIncreasePerFood = 0.1f;
        [SerializeField] private int scorePerFood = 10;

        [Header("References")]
        [SerializeField] private SnakeController snakeController;
        [SerializeField] private FoodSpawner foodSpawner;
        [SerializeField] private GridManager gridManager;

        // Game State
        private GameState currentState = GameState.Menu;
        private int currentScore = 0;
        private float currentSpeed;
        private float playTime = 0f;
        private int speedLevel = 1;
        private int highScore = 0;

        private const string HIGH_SCORE_KEY = "SnakeVR_HighScore";

        // Effect modifiers
        private float pointMultiplier = 1f;
        private float gameTimeScale = 1f;
        private float savedFixedDeltaTime;

        // Events
        public System.Action<int> OnScoreChanged;
        public System.Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Load high score from PlayerPrefs
            highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);

            // Save default fixed delta time
            savedFixedDeltaTime = Time.fixedDeltaTime;
        }

        private void Start()
        {
            currentSpeed = initialSpeed;
            // Start in menu state - UI will handle showing main menu
            ChangeState(GameState.Menu);
        }

        private void Update()
        {
            // Track play time during gameplay
            if (currentState == GameState.Playing)
            {
                playTime += Time.deltaTime;
            }
        }

        public void StartGame()
        {
            currentScore = 0;
            currentSpeed = initialSpeed;
            playTime = 0f;
            speedLevel = 1;
            pointMultiplier = 1f;
            gameTimeScale = 1f;
            OnScoreChanged?.Invoke(currentScore);

            if (snakeController != null)
            {
                snakeController.ResetSnake();
                snakeController.SetSpeed(currentSpeed);
            }

            if (foodSpawner != null)
            {
                foodSpawner.SpawnFood();
            }

            ChangeState(GameState.Playing);
        }

        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                ChangeState(GameState.Paused);
            }
            else if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }

        public void GameOver()
        {
            // Clear all active effects
            if (SpecialFoodManager.Instance != null)
            {
                SpecialFoodManager.Instance.ClearAllEffects();
            }

            // Reposition player to starting position
            // Find SnakeController if not assigned
            if (snakeController == null)
            {
                snakeController = FindObjectOfType<SnakeController>();
            }
            if (snakeController != null)
            {
                snakeController.RepositionToStart();
            }

            ChangeState(GameState.GameOver);
            Debug.Log($"Game Over! Final Score: {currentScore}");
        }

        public void RestartGame()
        {
            // Reset time scale before reloading
            Time.timeScale = 1f;
            Time.fixedDeltaTime = savedFixedDeltaTime;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnFoodEaten()
        {
            OnFoodEaten(1f);
        }

        public void OnFoodEaten(float foodPointMultiplier)
        {
            // Calculate score with both food-specific and active effect multipliers
            float totalMultiplier = foodPointMultiplier * pointMultiplier;
            int pointsEarned = Mathf.RoundToInt(scorePerFood * totalMultiplier);

            currentScore += pointsEarned;
            OnScoreChanged?.Invoke(currentScore);

            // Increase speed
            currentSpeed += speedIncreasePerFood;
            speedLevel = Mathf.FloorToInt((currentSpeed - initialSpeed) / speedIncreasePerFood) + 1;

            if (snakeController != null)
            {
                snakeController.SetSpeed(currentSpeed);
            }

            // Update UI speed display
            if (UI.UIManager.Instance != null)
            {
                UI.UIManager.Instance.UpdateSpeedLevel(speedLevel);
            }

            // Spawn new food
            if (foodSpawner != null)
            {
                foodSpawner.SpawnFood();
            }

            Debug.Log($"Score: {currentScore} (+{pointsEarned}), Speed: {currentSpeed}, Level: {speedLevel}");
        }

        public void SetTimeScale(float scale)
        {
            gameTimeScale = scale;

            // Only apply if game is playing
            if (currentState == GameState.Playing)
            {
                Time.timeScale = scale;
                Time.fixedDeltaTime = savedFixedDeltaTime * scale;
            }

            Debug.Log($"Time scale set to {scale}");
        }

        public void SetPointMultiplier(float multiplier)
        {
            pointMultiplier = multiplier;
            Debug.Log($"Point multiplier set to {multiplier}");
        }

        public float GetPointMultiplier()
        {
            return pointMultiplier;
        }

        private void ChangeState(GameState newState)
        {
            currentState = newState;
            OnGameStateChanged?.Invoke(currentState);

            switch (currentState)
            {
                case GameState.Menu:
                    Time.timeScale = 0f;
                    break;
                case GameState.Playing:
                    // Apply current game time scale (for slow-mo effect)
                    Time.timeScale = gameTimeScale;
                    Time.fixedDeltaTime = savedFixedDeltaTime * gameTimeScale;
                    break;
                case GameState.Paused:
                    Time.timeScale = 0f;
                    break;
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;
            }
        }

        public GameState GetCurrentState()
        {
            return currentState;
        }

        public int GetCurrentScore()
        {
            return currentScore;
        }

        public int GetHighScore()
        {
            return highScore;
        }

        public float GetPlayTime()
        {
            return playTime;
        }

        public int GetSpeedLevel()
        {
            return speedLevel;
        }

        public int GetSnakeLength()
        {
            if (snakeController != null)
            {
                return snakeController.GetSegmentCount() + 1; // +1 for head
            }
            return 1;
        }

        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                ChangeState(GameState.Playing);
            }
        }

        public void ReturnToMenu()
        {
            ChangeState(GameState.Menu);
        }

        public void SaveHighScore()
        {
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt(HIGH_SCORE_KEY, highScore);
                PlayerPrefs.Save();
                Debug.Log($"New high score saved: {highScore}");
            }
        }
    }
}
