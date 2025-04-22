using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    [Header("Game menu elements")]
    [SerializeField] private GameObject victoryPopupGO;

    private GameObject victoryPopup;
    private GameObject activeCanvasGO;

    // Start is called before the first frame update
    void Start()
    {
        victoryPopup = Instantiate(victoryPopupGO);
    }

    public void OpenVictoryPanel()
    {

    }
}
