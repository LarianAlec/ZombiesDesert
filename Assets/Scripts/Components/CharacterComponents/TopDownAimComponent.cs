using System;
using UnityEngine;

public class TopDownAimComponent : MonoBehaviour
{
    private PlayerCharacter player;
    private Vector2 aimInput;

    [Header("Aim info")]
    [SerializeField] private Transform aimTarget;
    [SerializeField] private LayerMask aimLayerMasks;
    [SerializeField] private float YTargetOffset = 0.0f;
    [SerializeField] private bool isAimingPrecisely = false;
    [SerializeField] private bool isTargetLockingEnabled = false;

    [Header("Camera info")]
    [SerializeField] private Transform cam;
    [SerializeField] private float YOffsetCameraPosition = 1.0f;
    [Range(1.5f, 2.5f)]
    [SerializeField] private float maxCameraDistance;
    [Range(0.2f, 0.7f)]
    [SerializeField] private float minCameraDistance;
    [Range(2.0f, 6.0f)]
    [SerializeField] private float cameraSensitivity;

    private RaycastHit lastKnownMouseHit;
    private Vector3 lastKnownMousePosition;


    private void Start()
    {
        player = GetComponent<PlayerCharacter>();
    }

    private void Update()
    {
        if (player.health.isDead)
            return;

        UpdateAimTargetPosition();
        UpdateCameraPosition();
    }

    public bool IsAimingPrecisely() => isAimingPrecisely;

    public bool IsTargetLockingEnabled() => isTargetLockingEnabled;


    public void SetAimInput(Vector2 inputVector)
    {
        aimInput = inputVector;
    }
    
    public void SetPreciselyAiming(bool value)
    {
        isAimingPrecisely = value;
    }

    public void EnableTargetLocking()
    {
        isTargetLockingEnabled = true;
    }

    public void DisableTargetLocking()
    {
        isTargetLockingEnabled = false;
    }

    public Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMasks))
        {
            lastKnownMousePosition = hitInfo.point;
            return hitInfo.point;
        }

        return lastKnownMousePosition;
    }

    public RaycastHit GetMouseHitInfo()
    {
        Ray ray = Camera.main.ScreenPointToRay(aimInput);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMasks))
        {
            lastKnownMouseHit = hitInfo;
            return hitInfo;
        }
        return lastKnownMouseHit;
    }

    public Transform GetTarget()
    {
        Transform target = null;
        Transform hittedObj = GetMouseHitInfo().transform;

        if (hittedObj != null)
        {
            // try to get AimLocableComponent from hitted obj
            if (hittedObj.GetComponent<AimLockable>() != null)
            {
                target = GetMouseHitInfo().transform;
            }
        }

        return target;
    }

    private void UpdateAimTargetPosition()
    {
        if (ShouldLockToTarget())
        {
            return;
        }

        RaycastHit hitInfo = GetMouseHitInfo();

        // if mouseHitInfo is null then target position in front of character
        if (hitInfo.transform == null)
        {
            Vector3 playerPosition = gameObject.transform.position;
            aimTarget.position = new Vector3(playerPosition.x, playerPosition.y + YTargetOffset, playerPosition.z) + Vector3.forward;
            return;
        }


        if (!isAimingPrecisely)
        {
            aimTarget.position = new Vector3(hitInfo.point.x, transform.position.y + YTargetOffset, hitInfo.point.z);
        }
        else
        {
            aimTarget.position = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
        }
    }

    private bool ShouldLockToTarget()
    {
        Transform targetTransform = GetTarget();
        if (targetTransform != null &&  isTargetLockingEnabled) 
        {
            aimTarget.position = targetTransform.position;
            return true;
        }
        return false;
    }

    public Vector3 GetAimDirection()
    {
        Vector3 muzzleSocketPosition = player.GetEquipmentComponent().GetCurrentEquippedWeapon().muzzleSocket.position;
        Vector3 aimDirection = (aimTarget.position - muzzleSocketPosition).normalized;
        return aimDirection;
    }

    private Vector3 DesiredCameraPosition()
    {
        Vector3 mousePos = GetMousePosition();
        Vector3 lookDirection = (mousePos - transform.position).normalized;

        float distanceToMousePos = Vector3.Distance(transform.position, mousePos);
        float clampDistance = Mathf.Clamp(distanceToMousePos, minCameraDistance, maxCameraDistance);

        Vector3 resultCamPos = transform.position + lookDirection * clampDistance;
        resultCamPos.y += YOffsetCameraPosition;

        return resultCamPos;
    }

    private void UpdateCameraPosition()
    {
        cam.position = Vector3.Lerp(cam.position, DesiredCameraPosition(), cameraSensitivity * Time.deltaTime);
    }
}
