using System.Collections;
using UnityEngine;
using TMPro;

namespace SnakeVR.UI
{
    /// <summary>
    /// Game over screen with stats summary and Play Again / Quit to Menu buttons.
    /// </summary>
    public class GameOverScreen : ArcadeMenuBase
    {
        [Header("Game Over Screen")]
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI lengthReachedText;
        [SerializeField] private TextMeshProUGUI timeSurvivedText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI newHighScoreText;
        [SerializeField] private ArcadeMenuButton playAgainButton;
        [SerializeField] private ArcadeMenuButton quitToMenuButton;

        [Header("Animation")]
        [SerializeField] private float statRevealDelay = 0.3f;
        [SerializeField] private float headerFlickerDuration = 1f;
        [SerializeField] private int headerFlickerCount = 5;

        [Header("Colors")]
        [SerializeField] private Color headerColor = new Color(1f, 0f, 0f); // Red
        [SerializeField] private Color scoreColor = new Color(1f, 0.84f, 0f); // Yellow
        [SerializeField] private Color lengthColor = new Color(0f, 1f, 0f); // Green
        [SerializeField] private Color timeColor = new Color(0f, 1f, 1f); // Cyan
        [SerializeField] private Color highScoreColor = new Color(1f, 0f, 1f); // Magenta

        private bool isNewHighScore = false;
        private float rainbowTime = 0f;

        protected override void Awake()
        {
            base.Awake();

            // Apply colors
            if (headerText != null) headerText.color = headerColor;
            if (finalScoreText != null) finalScoreText.color = scoreColor;
            if (lengthReachedText != null) lengthReachedText.color = lengthColor;
            if (timeSurvivedText != null) timeSurvivedText.color = timeColor;
            if (highScoreText != null) highScoreText.color = highScoreColor;

            // Set up button callbacks
            if (playAgainButton != null)
            {
                playAgainButton.OnClick.AddListener(OnPlayAgain);
                playAgainButton.SetColor(new Color(1f, 0.84f, 0f)); // Yellow
            }

            if (quitToMenuButton != null)
            {
                quitToMenuButton.OnClick.AddListener(OnQuitToMenu);
                quitToMenuButton.SetColor(Color.white);
            }

            // Hide new high score text initially
            if (newHighScoreText != null)
            {
                newHighScoreText.gameObject.SetActive(false);
            }
        }

        protected override void Update()
        {
            base.Update();

            // Animate new high score text with rainbow colors
            if (isNewHighScore && newHighScoreText != null && newHighScoreText.gameObject.activeSelf)
            {
                rainbowTime += Time.unscaledDeltaTime * 3f;
                float hue = Mathf.Repeat(rainbowTime, 1f);
                newHighScoreText.color = Color.HSVToRGB(hue, 1f, 1f);
            }
        }

        public override void Show()
        {
            // Hide all stats initially
            HideAllStats();

            base.Show();

            // Start animated reveal
            StartCoroutine(RevealStats());
        }

        private void HideAllStats()
        {
            if (finalScoreText != null) finalScoreText.gameObject.SetActive(false);
            if (lengthReachedText != null) lengthReachedText.gameObject.SetActive(false);
            if (timeSurvivedText != null) timeSurvivedText.gameObject.SetActive(false);
            if (highScoreText != null) highScoreText.gameObject.SetActive(false);
            if (newHighScoreText != null) newHighScoreText.gameObject.SetActive(false);
            if (playAgainButton != null) playAgainButton.gameObject.SetActive(false);
            if (quitToMenuButton != null) quitToMenuButton.gameObject.SetActive(false);
        }

        private IEnumerator RevealStats()
        {
            // Flicker header
            yield return StartCoroutine(FlickerHeader());

            if (GameManager.Instance == null) yield break;

            int finalScore = GameManager.Instance.GetCurrentScore();
            int length = GameManager.Instance.GetSnakeLength();
            float time = GameManager.Instance.GetPlayTime();
            int previousHighScore = GameManager.Instance.GetHighScore();

            // Check for new high score (before saving)
            isNewHighScore = finalScore > previousHighScore;

            // Save high score
            GameManager.Instance.SaveHighScore();
            int currentHighScore = GameManager.Instance.GetHighScore();

            // Show new high score banner if achieved
            if (isNewHighScore && newHighScoreText != null)
            {
                newHighScoreText.text = "NEW HIGH SCORE!";
                newHighScoreText.gameObject.SetActive(true);
                rainbowTime = 0f;
                yield return new WaitForSecondsRealtime(statRevealDelay);
            }

            // Reveal final score
            if (finalScoreText != null)
            {
                finalScoreText.text = $"FINAL SCORE\n{finalScore}";
                finalScoreText.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(statRevealDelay);
            }

            // Reveal length
            if (lengthReachedText != null)
            {
                lengthReachedText.text = $"LENGTH REACHED\n{length} SEGMENTS";
                lengthReachedText.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(statRevealDelay);
            }

            // Reveal time
            if (timeSurvivedText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);
                timeSurvivedText.text = $"TIME SURVIVED\n{minutes:D2}:{seconds:D2}";
                timeSurvivedText.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(statRevealDelay);
            }

            // Reveal high score
            if (highScoreText != null)
            {
                highScoreText.text = $"HIGH SCORE\n{currentHighScore}";
                highScoreText.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(statRevealDelay);
            }

            // Show buttons
            if (playAgainButton != null) playAgainButton.gameObject.SetActive(true);
            if (quitToMenuButton != null) quitToMenuButton.gameObject.SetActive(true);
        }

        private IEnumerator FlickerHeader()
        {
            if (headerText == null) yield break;

            float flickerInterval = headerFlickerDuration / (headerFlickerCount * 2);

            for (int i = 0; i < headerFlickerCount; i++)
            {
                headerText.gameObject.SetActive(false);
                yield return new WaitForSecondsRealtime(flickerInterval);
                headerText.gameObject.SetActive(true);
                yield return new WaitForSecondsRealtime(flickerInterval);
            }
        }

        private void OnPlayAgain()
        {
            Hide();
            isNewHighScore = false;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.StartCountdown(() =>
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.StartGame();
                    }
                });
            }
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
        }

        private void OnQuitToMenu()
        {
            Hide();
            isNewHighScore = false;

            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowMainMenu();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReturnToMenu();
            }
        }
    }
}
