using UnityEngine;
using TMPro;

namespace SnakeVR.UI
{
    /// <summary>
    /// Main menu with Start Game and Quit buttons.
    /// </summary>
    public class MainMenu : ArcadeMenuBase
    {
        [Header("Main Menu")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private ArcadeMenuButton startButton;
        [SerializeField] private ArcadeMenuButton quitButton;

        [Header("Title Animation")]
        [SerializeField] private bool animateTitle = true;
        [SerializeField] private float colorCycleSpeed = 2f;

        private float colorCycleTime = 0f;
        private Color[] rainbowColors = new Color[]
        {
            new Color(1f, 0f, 0f),       // Red
            new Color(1f, 0.5f, 0f),     // Orange
            new Color(1f, 1f, 0f),       // Yellow
            new Color(0f, 1f, 0f),       // Green
            new Color(0f, 1f, 1f),       // Cyan
            new Color(0f, 0f, 1f),       // Blue
            new Color(1f, 0f, 1f),       // Magenta
        };

        protected override void Awake()
        {
            base.Awake();

            // Set up button callbacks
            if (startButton != null)
            {
                startButton.OnClick.AddListener(OnStartGame);
                startButton.SetColor(new Color(1f, 0.84f, 0f)); // Yellow
            }

            if (quitButton != null)
            {
                quitButton.OnClick.AddListener(OnQuit);
                quitButton.SetColor(Color.white);
            }
        }

        protected override void Update()
        {
            base.Update();

            // Animate title with rainbow colors
            if (animateTitle && titleText != null)
            {
                colorCycleTime += Time.unscaledDeltaTime * colorCycleSpeed;
                int index = Mathf.FloorToInt(colorCycleTime) % rainbowColors.Length;
                int nextIndex = (index + 1) % rainbowColors.Length;
                float t = colorCycleTime - Mathf.Floor(colorCycleTime);

                titleText.color = Color.Lerp(rainbowColors[index], rainbowColors[nextIndex], t);
            }
        }

        private void OnStartGame()
        {
            Hide();

            // Start countdown then game
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

        private void OnQuit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        public override void Show()
        {
            base.Show();
            colorCycleTime = 0f;
        }
    }
}
