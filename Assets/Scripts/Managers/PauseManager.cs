using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public bool isPaused { get; private set; }

    [Header("Components to deactivate when paused")]
    [SerializeField] private TopDownAimComponent topDownAimComponent;
    [SerializeField] private CharacterEquipmentComponent characterEquipmentComponent;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private CameraMovementComponent cameraMovementComponent;

    // Components to deactivate when paused
    private List<MonoBehaviour> components;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        components = new List<MonoBehaviour> { topDownAimComponent, characterEquipmentComponent, playerController, cameraMovementComponent };
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0.0f;
        foreach (MonoBehaviour component in components)
        {
            component.enabled = false;
        }
    }

    public void Unpause()
    {
        isPaused = false;
        Time.timeScale = 1.0f;
        foreach (MonoBehaviour component in components)
        {
            component.enabled = true;
        }
    }

}
