using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovementComponent : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;

    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float maxRotationSpeed = 1f;
    private bool isLocked = true;

    private void Start()
    {
        if (cameraTransform == null)
        {
            cameraTransform = FindObjectOfType<CinemachineVirtualCamera>().GetComponent<Transform>();
        }

        playerControls = GetComponent<PlayerController>().controls;

        AssignInputEvents();
    }

    private void OnEnable()
    {
        StartCoroutine(AssignInputNextFrame());
    }

    private void OnDisable()
    {
        playerControls.Camera.RotateCamera.performed -= RotateCamera;
    }

    private void AssignInputEvents()
    {
        playerControls.Camera.RotateCamera.performed += RotateCamera;

        playerControls.Camera.UnlockCamera.performed += context =>
        {
            isLocked = false;
        };

        playerControls.Camera.UnlockCamera.canceled += context =>
        {
            isLocked = true;
        };
    }

    private void RotateCamera(InputAction.CallbackContext context)
    {
        if (isLocked)
            return;

        float value = context.ReadValue<Vector2>().x;
        cameraTransform.rotation = Quaternion.Euler(cameraTransform.rotation.eulerAngles.x, value * maxRotationSpeed + cameraTransform.rotation.eulerAngles.y, cameraTransform.rotation.eulerAngles.z);
    }

    IEnumerator AssignInputNextFrame()
    {
        yield return new WaitForEndOfFrame();
        playerControls.Camera.RotateCamera.performed += RotateCamera;
    }
}
