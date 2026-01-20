using UnityEngine;
using TMPro;

namespace SnakeVR.UI
{
    /// <summary>
    /// Pause menu with Resume, Restart, and Quit to Menu buttons.
    /// </summary>
    public class PauseMenu : ArcadeMenuBase
    {
        [Header("Pause Menu")]
        [SerializeField] private TextMeshProUGUI headerText;
        [SerializeField] private TextMeshProUGUI statsText;
        [SerializeField] private ArcadeMenuButton resumeButton;
        [SerializeField] private ArcadeMenuButton restartButton;
        [SerializeField] private ArcadeMenuButton quitToMenuButton;

        [Header("Header Animation")]
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private float pulseMin = 0.8f;
        [SerializeField] private float pulseMax = 1f;

        private float pulseTime = 0f;
        private Vector3 originalHeaderScale;

        protected override void Awake()
        {
            base.Awake();

            if (headerText != null)
            {
                headerText.color = new Color(0f, 1f, 1f); // Cyan
                originalHeaderScale = headerText.transform.localScale;
            }

            // Set up button callbacks
            if (resumeButton != null)
            {
                resumeButton.OnClick.AddListener(OnResume);
                resumeButton.SetColor(new Color(0f, 1f, 0f)); // Green
            }

            if (restartButton != null)
            {
                restartButton.OnClick.AddListener(OnRestart);
                restartButton.SetColor(new Color(1f, 0.84f, 0f)); // Yellow
            }

            if (quitToMenuButton != null)
            {
                quitToMenuButton.OnClick.AddListener(OnQuitToMenu);
                quitToMenuButton.SetColor(Color.white);
            }
        }

        protected override void Update()
        {
            base.Update();

            // Pulse header animation
            if (headerText != null)
            {
                pulseTime += Time.unscaledDeltaTime * pulseSpeed;
                float scale = Mathf.Lerp(pulseMin, pulseMax, (Mathf.Sin(pulseTime * Mathf.PI) + 1f) / 2f);
                headerText.transform.localScale = originalHeaderScale * scale;
            }
        }

        public override void Show()
        {
            base.Show();
            UpdateStats();
            pulseTime = 0f;
        }

        private void UpdateStats()
        {
            if (statsText != null && GameManager.Instance != null)
            {
                int score = GameManager.Instance.GetCurrentScore();
                int length = GameManager.Instance.GetSnakeLength();
                float time = GameManager.Instance.GetPlayTime();
                int minutes = Mathf.FloorToInt(time / 60f);
                int seconds = Mathf.FloorToInt(time % 60f);

                statsText.text = $"SCORE: {score} | LENGTH: {length} | TIME: {minutes:D2}:{seconds:D2}";
            }
        }

        private void OnResume()
        {
            Hide();

            // Resume with countdown
            if (UIManager.Instance != null)
            {
                UIManager.Instance.StartCountdown(() =>
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.ResumeGame();
                    }
                });
            }
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }

        private void OnRestart()
        {
            Hide();

            if (UIManager.Instance != null)
            {
                UIManager.Instance.StartCountdown(() =>
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.RestartGame();
                    }
                });
            }
            else if (GameManager.Instance != null)
            {
                GameManager.Instance.RestartGame();
            }
        }

        private void OnQuitToMenu()
        {
            Hide();

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
