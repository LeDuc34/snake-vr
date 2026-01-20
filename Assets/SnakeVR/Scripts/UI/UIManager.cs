using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace SnakeVR.UI
{
    /// <summary>
    /// Central UI manager that orchestrates all arcade UI elements.
    /// Handles game state transitions and countdown display.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private ArcadeHUD hud;
        [SerializeField] private MainMenu mainMenu;
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private GameOverScreen gameOverScreen;

        [Header("Countdown")]
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private float countdownInterval = 1f;
        [SerializeField] private Color countdownColor = new Color(1f, 0.84f, 0f); // Yellow
        [SerializeField] private Color goColor = new Color(0f, 1f, 0f); // Green

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip countdownBeep;
        [SerializeField] private AudioClip goSound;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            // Create audio source if not assigned
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 0f;
            }

            // Set countdown text color
            if (countdownText != null)
            {
                countdownText.color = countdownColor;
                countdownText.gameObject.SetActive(false);
            }
        }

        private void Start()
        {
            // Subscribe to game state changes
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
            }

            // Initial state - show main menu
            HideAllUI();
            ShowMainMenu();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnGameStateChanged -= OnGameStateChanged;
            }
        }

        private void OnGameStateChanged(GameState newState)
        {
            switch (newState)
            {
                case GameState.Menu:
                    HideAllUI();
                    ShowMainMenu();
                    break;

                case GameState.Playing:
                    HideAllMenus();
                    ShowHUD();
                    break;

                case GameState.Paused:
                    HideHUD();
                    ShowPauseMenu();
                    break;

                case GameState.GameOver:
                    HideHUD();
                    ShowGameOverScreen();
                    break;
            }
        }

        private void HideAllUI()
        {
            HideHUD();
            HideAllMenus();
            HideCountdown();
        }

        private void HideAllMenus()
        {
            if (mainMenu != null) mainMenu.Hide();
            if (pauseMenu != null) pauseMenu.Hide();
            if (gameOverScreen != null) gameOverScreen.Hide();
        }

        private void HideHUD()
        {
            if (hud != null) hud.gameObject.SetActive(false);
        }

        private void ShowHUD()
        {
            if (hud != null)
            {
                hud.gameObject.SetActive(true);
                hud.UpdateAllDisplays();
            }
        }

        public void ShowMainMenu()
        {
            HideAllMenus();
            if (mainMenu != null) mainMenu.Show();
        }

        private void ShowPauseMenu()
        {
            HideAllMenus();
            if (pauseMenu != null) pauseMenu.Show();
        }

        private void ShowGameOverScreen()
        {
            HideAllMenus();
            if (gameOverScreen != null) gameOverScreen.Show();
        }

        private void HideCountdown()
        {
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Shows a 3-2-1-GO countdown before invoking the callback.
        /// </summary>
        public void StartCountdown(Action onComplete)
        {
            StartCoroutine(CountdownCoroutine(onComplete));
        }

        private IEnumerator CountdownCoroutine(Action onComplete)
        {
            if (countdownText == null)
            {
                // No countdown text, just invoke callback
                onComplete?.Invoke();
                yield break;
            }

            countdownText.gameObject.SetActive(true);
            Vector3 originalScale = countdownText.transform.localScale;

            // 3
            countdownText.text = "3";
            countdownText.color = countdownColor;
            PlayCountdownBeep();
            yield return StartCoroutine(PulseText(countdownText, originalScale, countdownInterval));

            // 2
            countdownText.text = "2";
            PlayCountdownBeep();
            yield return StartCoroutine(PulseText(countdownText, originalScale, countdownInterval));

            // 1
            countdownText.text = "1";
            PlayCountdownBeep();
            yield return StartCoroutine(PulseText(countdownText, originalScale, countdownInterval));

            // GO!
            countdownText.text = "GO!";
            countdownText.color = goColor;
            PlayGoSound();
            yield return StartCoroutine(PulseText(countdownText, originalScale, countdownInterval * 0.5f));

            // Hide and invoke callback
            countdownText.gameObject.SetActive(false);
            countdownText.transform.localScale = originalScale;

            onComplete?.Invoke();
        }

        private IEnumerator PulseText(TextMeshProUGUI text, Vector3 originalScale, float duration)
        {
            float elapsed = 0f;
            float startScale = 1.5f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;

                // Scale from large to normal
                float scale = Mathf.Lerp(startScale, 1f, t);
                text.transform.localScale = originalScale * scale;

                // Fade in quickly then hold
                float alpha = Mathf.Min(1f, t * 4f);
                Color c = text.color;
                c.a = alpha;
                text.color = c;

                yield return null;
            }

            text.transform.localScale = originalScale;
        }

        private void PlayCountdownBeep()
        {
            if (audioSource != null && countdownBeep != null)
            {
                audioSource.PlayOneShot(countdownBeep);
            }
        }

        private void PlayGoSound()
        {
            if (audioSource != null && goSound != null)
            {
                audioSource.PlayOneShot(goSound);
            }
        }

        /// <summary>
        /// Update the HUD length display (called from SnakeController).
        /// </summary>
        public void UpdateSnakeLength(int length)
        {
            if (hud != null)
            {
                hud.UpdateLengthDisplay(length);
            }
        }

        /// <summary>
        /// Update the HUD speed display (called from GameManager).
        /// </summary>
        public void UpdateSpeedLevel(int level)
        {
            if (hud != null)
            {
                hud.UpdateSpeedDisplay(level);
            }
        }
    }
}
