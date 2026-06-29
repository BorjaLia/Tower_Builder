using UnityEngine;
using UnityEngine.SceneManagement;

// Error: Todos los botones se setea el action por inspector!
// Warning: Todos los if están inline. Poner debajo la instrucción dento del if.
public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    private const string GAMEPLAY_SCENE_NAME = "GameplayScene";

    private void Start()
    {
        // Warning: Inline:...
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

    // Error: la consigna pide ocultar/deshabilitar el boton Exit en WebGL. Aqui no hay guarda #if UNITY_WEBGL ni codigo que oculte el boton, y Application.Quit() no tiene efecto en WebGL.
    public void ExitGame()
    {
        Application.Quit();
    }
}