using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Canvas Objects")]
    [SerializeField] private GameObject mainMenuCanvasGO;
    [SerializeField] private GameObject settingsCanvasGO;
    [SerializeField] private GameObject soundCanvasGO;

    [Header("First Selected Options")]
    [SerializeField] private GameObject firstSelectedButtonForMainMenu;
    [SerializeField] private GameObject firstSelectedButtonForSettingsMenu;
    [SerializeField] private GameObject firstSelectedButtonForSoundSettingsMenu;

    private GameObject activeCanvasGO;

    private void Start()
    {
        gameObject.SetActive(false);
        settingsCanvasGO.SetActive(false);

        // by default
        activeCanvasGO = mainMenuCanvasGO;
    }

    public void OpenMainMenu()
    {
        ToggleCanvasGO(mainMenuCanvasGO);
    }

    #region Callback Canvas Open Functions

    private void OnMainMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForMainMenu);
    }

    private void OnSettingsMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForSettingsMenu);
    }

    private void OnSoundMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForSoundSettingsMenu);
    }

    #endregion

    #region Button/Slider Actions

    // -------------- Main Menu Buttons -------------
    public void OnMainMenuResumePress()
    {
        UI_Manager.instance.CloseMainMenu();
    }

    public void OnMainMenuSettingsPress()
    {
        ToggleCanvasGO(settingsCanvasGO);
    }

    public void OnMainMenuExitPress()
    {
        Application.Quit();
    }


    // ---------------- Setting Menu Buttons -------------
    public void OnSettingsMenuSoundPress()
    {
        ToggleCanvasGO(soundCanvasGO);
    }

    public void OnSettingsMenuBackPress()
    {
        ToggleCanvasGO(mainMenuCanvasGO);
    }

    // ---------------- Sound Menu Buttons ---------------

    public void OnSoundSettingsBackPress()
    {
        ToggleCanvasGO(settingsCanvasGO);
    }

    #endregion


    private void ToggleCanvasGO(GameObject canvasToToggleGO)
    {
        activeCanvasGO.SetActive(false);
        activeCanvasGO = canvasToToggleGO;
        activeCanvasGO.SetActive(true);

        if (activeCanvasGO == mainMenuCanvasGO) 
        {
            OnMainMenuOpened();
        }
        else if (activeCanvasGO == settingsCanvasGO)
        {
            OnSettingsMenuOpened();
        }
        else if (activeCanvasGO == soundCanvasGO)
        {
            OnSoundMenuOpened();
        }
        else
        {
            // do nothing
        }
    }
}
