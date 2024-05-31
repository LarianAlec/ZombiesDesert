using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [Header("Loading properties")]
    [SerializeField] private Scene sceneToLoad;
    [SerializeField] private Scene sceneGYM;
    [SerializeField] private GameObject loadingBarGO;
    [SerializeField] private GameObject[] objectsToHide;

    [Header("Menu Canvas Objects")]
    [SerializeField] private GameObject startMenuContentGO;
    [SerializeField] private GameObject optionsMenuContentGO;
    [SerializeField] private GameObject graphicsMenuContentGO;
    [SerializeField] private GameObject audioMenuContentGO;
    [SerializeField] private GameObject controlsMenuContentGO;

    [Header("First Selected Options")]
    [SerializeField] private GameObject firstSelectedButtonForStartMenu;
    [SerializeField] private GameObject firstSelectedButtonForOptionsMenu;
    [SerializeField] private GameObject firstSelectedButtonForGraphicsMenu;
    [SerializeField] private GameObject firstSelectedButtonForAudioMenu;
    [SerializeField] private GameObject firstSelectedButtonForControlsMenu;

    private GameObject activeGO;

    private void Awake()
    {
        loadingBarGO.SetActive(false);
        
        // by default
        activeGO = startMenuContentGO;
    }

    private void StartGame()
    {
        HideMenu();

        if (sceneToLoad != null)
        {
            SceneManager.LoadScene(sceneToLoad.GetHashCode(), LoadSceneMode.Single);
        }

        //update the loading bar
    }

    private void HideMenu()
    {
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            objectsToHide[i].SetActive(false);
        }
    }

    #region Button/Slider Actions

    // ------------ Start Menu Actions ----------
    public void OnStartMenyGYMButtonPressed()
    {
        SceneManager.LoadScene("GYM", LoadSceneMode.Single); 
    }

    public void OnStartMenuStartButtonPressed()
    {
        StartGame();
    }

    public void OnStartMenuOptionsButtonPressed()
    {
        ToggleContent(optionsMenuContentGO);
    }

    public void OnStartMenuExitButtonPressed()
    {
        Application.Quit();
    }

    // ------------- Option Menu Actions -----------
    public void OnOptionsMenuGraphicsButtonPressed()
    {
        ToggleContent(graphicsMenuContentGO);
    }
    public void OnOptionsMenuAudioButtonPressed()
    {
        ToggleContent(audioMenuContentGO);
    }
    public void OnOptionsMenuControlButtonPressed()
    {
        ToggleContent(controlsMenuContentGO);
    }
    public void OnOptionsMenuReturnButtonPressed()
    {
        ToggleContent(startMenuContentGO);
    }

    // ------------- Graphics Menu Actions ---------
    public void OnGraphicsMenuReturnButtonPressed()
    {
        ToggleContent(optionsMenuContentGO);
    }

    // ------------- Audio Menu Actions -----------
    public void OnAudioMenuReturnButtonPressed()
    {
        ToggleContent(optionsMenuContentGO);
    }

    // ------------- Control Menu Actions ---------
    public void OnControlMenuReturnButtonPressed()
    {
        ToggleContent(optionsMenuContentGO);
    }

    #endregion

    private void ToggleContent(GameObject contentToToggleGO)
    {
        activeGO.SetActive(false);
        activeGO = contentToToggleGO;
        activeGO.SetActive(true);

        if (activeGO == startMenuContentGO)
        {
            OnStartMenuOpened();
        }
        else if (activeGO == optionsMenuContentGO)
        {
            OnOptionsMenuOpened();
        }
        else if (activeGO == graphicsMenuContentGO)
        {
            OnGraphicsMenuOpened();
        }
        else if (activeGO == audioMenuContentGO)
        {
            OnAudioMenuOpened();
        }
        else if (activeGO == controlsMenuContentGO)
        {
            OnControlsMenuOpened();
        }
    }

    #region Open Canvas Callback Functions

    private void OnStartMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForStartMenu);
    }

    private void OnOptionsMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForOptionsMenu);
    }

    private void OnGraphicsMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForGraphicsMenu);
    }

    private void OnAudioMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForAudioMenu);
    }

    private void OnControlsMenuOpened()
    {
        EventSystem.current.SetSelectedGameObject(firstSelectedButtonForControlsMenu);
    }

    #endregion
}
