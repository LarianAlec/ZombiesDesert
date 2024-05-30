using Unity.VisualScripting;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvasGO;
    [SerializeField] private GameObject settingsCanvasGO;

    private void Start()
    {
        settingsCanvasGO.SetActive(false);
    }

    public void OpenMainMenu()
    {
        mainMenuCanvasGO.SetActive(true);
        settingsCanvasGO.SetActive(false);
    }

    public void CloseAllMenus() 
    {
        mainMenuCanvasGO.SetActive(false);
        settingsCanvasGO.SetActive(false);
    }
}
