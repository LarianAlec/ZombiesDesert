using Unity.VisualScripting;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvasGO;
    [SerializeField] private GameObject settingsCanvasGO;

    private void Start()
    {
        gameObject.SetActive(false);
        settingsCanvasGO.SetActive(false);
    }

    public void OpenMainMenu()
    {
        mainMenuCanvasGO.SetActive(true);
        settingsCanvasGO.SetActive(false);
    }


    #region Main Menu Buttons Actions

    public void OnMainMenuSettingsPress()
    {
        settingsCanvasGO.SetActive(true);
        mainMenuCanvasGO.SetActive(false);
    }

    public void OnMainMenuResumePress()
    {
        UI_Manager.instance.UntoggleMainMenu();
    }

    public void OnMainMenuExitPress()
    {
        Application.Quit();
    }

    public void OnSettingsBackPress()
    {
        settingsCanvasGO.SetActive(false);
        mainMenuCanvasGO.SetActive(true);
    }

    #endregion

}
