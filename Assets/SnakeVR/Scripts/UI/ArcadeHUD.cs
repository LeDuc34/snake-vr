using UnityEngine;
using TMPro;

namespace SnakeVR.UI
{
    /// <summary>
    /// Retro arcade-style HUD with corner clusters.
    /// Attach to a Canvas that is a child of the main camera.
    /// </summary>
    public class ArcadeHUD : MonoBehaviour
    {
        [Header("Text References")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI lengthText;
        [SerializeField] private TextMeshProUGUI speedText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI timeText;

        [Header("Colors")]
        [SerializeField] private Color scoreColor = new Color(1f, 0.84f, 0f);      // Yellow #FFD700
        [SerializeField] private Color lengthColor = new Color(0f, 1f, 0f);         // Green #00FF00
        [SerializeField] private Color speedColor = new Color(0f, 1f, 1f);          // Cyan #00FFFF
        [SerializeField] private Color highScoreColor = new Color(1f, 0f, 1f);      // Magenta #FF00FF
        [SerializeField] private Color timeColor = Color.white;

        [Header("Animation")]
        [SerializeField] private float scorePulseScale = 1.2f;
        [SerializeField] private float pulseDuration = 0.2f;

        // Cached values for change detection
        private int lastScore = -1;
        private int lastHighScore = -1;
        private float pulseTimer = 0f;
        private Vector3 originalScoreScale;

        private void Awake()
        {
            ApplyColors();
            if (scoreText != null)
            {
                originalScoreScale = scoreText.transform.localScale;
            }
        }

        private void Start()
        {
            // Subscribe to GameManager events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged += OnScoreChanged;
                GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }

            // Initial update
            UpdateAllDisplays();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged -= OnScoreChanged;
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        private void Update()
        {
            // Update time display continuously during gameplay
            if (GameManager.Instance != null && GameManager.Instance.GetCurrentState() == GameState.Playing)
            {
                UpdateTimeDisplay();
            }

            // Handle score pulse animation
            if (pulseTimer > 0)
            {
                pulseTimer -= Time.unscaledDeltaTime;
                float t = pulseTimer / pulseDuration;
                float scale = Mathf.Lerp(1f, scorePulseScale, t);
                if (scoreText != null)
                {
                    scoreText.transform.localScale = originalScoreScale * scale;
                }
            }
        }

        private void ApplyColors()
        {
            if (scoreText != null) scoreText.color = scoreColor;
            if (lengthText != null) lengthText.color = lengthColor;
            if (speedText != null) speedText.color = speedColor;
            if (highScoreText != null) highScoreText.color = highScoreColor;
            if (timeText != null) timeText.color = timeColor;
        }

        private void OnScoreChanged(int newScore)
        {
            UpdateScoreDisplay(newScore);

            // Trigger pulse animation
            if (newScore > lastScore && lastScore >= 0)
            {
                pulseTimer = pulseDuration;
            }
            lastScore = newScore;

            // Check for high score beat
            int highScore = GameManager.Instance.GetHighScore();
            if (newScore > highScore && lastHighScore < highScore)
            {
                // New high score achieved - could add special effect here
                UpdateHighScoreDisplay(newScore);
            }
        }

        private void OnGameStateChanged(GameState state)
        {
            // Show/hide HUD based on game state
            gameObject.SetActive(state == GameState.Playing);
        }

        public void UpdateAllDisplays()
        {
            if (GameManager.Instance == null) return;

            UpdateScoreDisplay(GameManager.Instance.GetCurrentScore());
            UpdateHighScoreDisplay(GameManager.Instance.GetHighScore());
            UpdateLengthDisplay(GameManager.Instance.GetSnakeLength());
            UpdateSpeedDisplay(GameManager.Instance.GetSpeedLevel());
            UpdateTimeDisplay();
        }

        private void UpdateScoreDisplay(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = $"SCORE {score:D5}";
            }
        }

        private void UpdateHighScoreDisplay(int highScore)
        {
            if (highScoreText != null)
            {
                highScoreText.text = $"HI-SCORE {highScore:D5}";
                lastHighScore = highScore;
            }
        }

        public void UpdateLengthDisplay(int length)
        {
            if (lengthText != null)
            {
                lengthText.text = $"LENGTH {length:D2}";
            }
        }

        public void UpdateSpeedDisplay(int speedLevel)
        {
            if (speedText != null)
            {
                speedText.text = $"SPEED LV.{speedLevel}";
            }
        }

        private void UpdateTimeDisplay()
        {
            if (timeText != null && GameManager.Instance != null)
            {
                float time = GameManager.Instance.GetPlayTime();
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeText.text = $"TIME {minutes:D2}:{seconds:D2}";
            }
        }
    }
}
