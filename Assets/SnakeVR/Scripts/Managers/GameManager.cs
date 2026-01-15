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
        }

        private void Start()
        {
            currentSpeed = initialSpeed;
            // Auto-start the game
            StartGame();
            // Game starts automatically in Playing state (StartGame already sets it)
        }

        public void StartGame()
        {
            currentScore = 0;
            currentSpeed = initialSpeed;
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
            ChangeState(GameState.GameOver);
            Debug.Log($"Game Over! Final Score: {currentScore}");
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void OnFoodEaten()
        {
            // Increase score
            currentScore += scorePerFood;
            OnScoreChanged?.Invoke(currentScore);

            // Increase speed
            currentSpeed += speedIncreasePerFood;
            if (snakeController != null)
            {
                snakeController.SetSpeed(currentSpeed);
            }

            // Spawn new food
            if (foodSpawner != null)
            {
                foodSpawner.SpawnFood();
            }

            Debug.Log($"Score: {currentScore}, Speed: {currentSpeed}");
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
                    Time.timeScale = 1f;
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
    }
}
