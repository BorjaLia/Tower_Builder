using UnityEngine;

public class PauseMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject pauseBackground;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        if (pauseBackground) pauseBackground.SetActive(false);
        if (pausePanel) pausePanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(false);

        GameplayManager.OnPauseToggled += HandlePauseStateChanged;
    }

    private void OnDestroy()
    {
        GameplayManager.OnPauseToggled -= HandlePauseStateChanged;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
            {
                CloseSettings();
            }
            else
            {
                GameplayManager.s_instance.TogglePause();
            }
        }
    }

    private void HandlePauseStateChanged(bool isPaused)
    {
        if (pauseBackground)
        {
            pauseBackground.SetActive(isPaused);
        }
        if (pausePanel)
        {
            pausePanel.SetActive(isPaused && (!settingsPanel || !settingsPanel.activeSelf));
        }
    }

    public void RetryGame()
    {
        GameplayManager.s_instance.RetryGame();
    }

    public void ResumeGame()
    {
        if (GameplayManager.s_instance.IsPaused)
        {
            GameplayManager.s_instance.TogglePause();
        }
    }

    public void OpenSettings()
    {
        if (pausePanel) pausePanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
        if (pausePanel) pausePanel.SetActive(true);
    }

    public void ExitToMenu()
    {
        GameplayManager.s_instance.ReturnToMenu();
    }
}