using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Canvas Objects")]
    [SerializeField] private GameObject mainMenuCanvasGO;
    [SerializeField] private GameObject settingsCanvasGO;

    [Header("First Selected Options")]
    [SerializeField] private GameObject firstSelectedButtonForMainMenu;
    [SerializeField] public GameObject firstSelectedButtonForSettingsMenu;

    private void Start()
    {
        gameObject.SetActive(false);
        settingsCanvasGO.SetActive(false);
    }

    #region Callback Canvas Open Functions

    public void OnMainMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForMainMenu);
    }

    public void OnSettingsMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForSettingsMenu);
    }

    #endregion

    #region Button Actions

    // -------------- Main Menu Buttons -------------
    public void OnMainMenuSettingsPress()
    {
        settingsCanvasGO.SetActive(true);
        mainMenuCanvasGO.SetActive(false);

        OnSettingsMenuOpened();
    }

    public void OnMainMenuResumePress()
    {
        UI_Manager.instance.CloseMainMenu();
    }

    public void OnMainMenuExitPress()
    {
        Application.Quit();
    }


    // ---------------- Setting Menu buttons -------------
    public void OnSettingsBackPress()
    {
        settingsCanvasGO.SetActive(false);
        mainMenuCanvasGO.SetActive(true);

        OnMainMenuOpened();
    }

    #endregion

}
