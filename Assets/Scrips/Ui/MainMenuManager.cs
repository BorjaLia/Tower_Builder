using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    private const string GAMEPLAY_SCENE_NAME = "GameplayScene";

    private void Start()
    {
        if (mainPanel) mainPanel.SetActive(true);
        if (settingsPanel) settingsPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(GAMEPLAY_SCENE_NAME);
    }

    public void OpenSettings()
    {
        if (mainPanel) mainPanel.SetActive(false);
        if (settingsPanel) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (mainPanel) mainPanel.SetActive(true);
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    public void OpenCredits()
    {
        if (mainPanel) mainPanel.SetActive(false);
        if (creditsPanel) creditsPanel.SetActive(true);
    }

    public void CloseCredits()
    {
        if (mainPanel) mainPanel.SetActive(true);
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}