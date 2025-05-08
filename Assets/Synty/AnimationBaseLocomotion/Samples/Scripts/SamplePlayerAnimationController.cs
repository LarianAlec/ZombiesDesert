using Synty.AnimationBaseLocomotion.Samples.InputSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Synty.AnimationBaseLocomotion.Samples
{
    public class SamplePlayerAnimationController : MonoBehaviour
    {
        #region Enum

        private enum AnimationState
        {
            Base,
            Locomotion,
            Jump,
            Fall,
            Crouch,
            WeaponAction
        }

        private enum GaitState
        {
            Idle,
            Walk,
            Run,
            Sprint
        }

        #endregion

        #region Animation Variable Hashes

        private readonly int _movementInputTappedHash = Animator.StringToHash("MovementInputTapped");
        private readonly int _movementInputPressedHash = Animator.StringToHash("MovementInputPressed");
        private readonly int _movementInputHeldHash = Animator.StringToHash("MovementInputHeld");
        private readonly int _shuffleDirectionXHash = Animator.StringToHash("ShuffleDirectionX");
        private readonly int _shuffleDirectionZHash = Animator.StringToHash("ShuffleDirectionZ");

        private readonly int _moveSpeedHash = Animator.StringToHash("MoveSpeed");
        private readonly int _currentGaitHash = Animator.StringToHash("CurrentGait");

        private readonly int _isJumpingAnimHash = Animator.StringToHash("IsJumping");
        private readonly int _fallingDurationHash = Animator.StringToHash("FallingDuration");

        private readonly int _inclineAngleHash = Animator.StringToHash("InclineAngle");

        private readonly int _strafeDirectionXHash = Animator.StringToHash("StrafeDirectionX");
        private readonly int _strafeDirectionZHash = Animator.StringToHash("StrafeDirectionZ");

        private readonly int _forwardStrafeHash = Animator.StringToHash("ForwardStrafe");
        private readonly int _cameraRotationOffsetHash = Animator.StringToHash("CameraRotationOffset");
        private readonly int _isStrafingHash = Animator.StringToHash("IsStrafing");
        private readonly int _isTurningInPlaceHash = Animator.StringToHash("IsTurningInPlace");

        private readonly int _isCrouchingHash = Animator.StringToHash("IsCrouching");

        private readonly int _isWalkingHash = Animator.StringToHash("IsWalking");
        private readonly int _isStoppedHash = Animator.StringToHash("IsStopped");
        private readonly int _isStartingHash = Animator.StringToHash("IsStarting");

        private readonly int _isGroundedHash = Animator.StringToHash("IsGrounded");

        private readonly int _leanValueHash = Animator.StringToHash("LeanValue");
        private readonly int _headLookXHash = Animator.StringToHash("HeadLookX");
        private readonly int _headLookYHash = Animator.StringToHash("HeadLookY");

        private readonly int _bodyLookXHash = Animator.StringToHash("BodyLookX");
        private readonly int _bodyLookYHash = Animator.StringToHash("BodyLookY");

        private readonly int _locomotionStartDirectionHash = Animator.StringToHash("LocomotionStartDirection");

        public readonly int _weaponTypeHash = Animator.StringToHash("WeaponType");
        private readonly int _isFiringHash = Animator.StringToHash("IsFiring");
        private readonly int _isReloadingHash = Animator.StringToHash("IsReloading");
        public readonly int _equipWeaponHash = Animator.StringToHash("EquipWeapon");
        public readonly int _unequipWeaponHash = Animator.StringToHash("UnequipWeapon");

        #endregion

        #region Player Settings Variables

        #region Scripts/Objects

        [Header("External Components")]
        [Tooltip("Script controlling camera behavior")]
        [SerializeField]
        private SampleCameraController _cameraController;
        [SerializeField]
        private Camera _thirdPersonCamera;
        [SerializeField]
        private Camera _topDownCamera;
        [Tooltip("InputReader handles player input")]
        [SerializeField]
        private InputReader _inputReader;
        [Tooltip("Animator component for controlling player animations")]
        [SerializeField]
        private Animator _animator;
        [Tooltip("Character Controller component for controlling player movement")]
        [SerializeField]
        private CharacterController _controller;
        [Tooltip("Character Equipment Component for acting with weapons")]
        [SerializeField]
        private CharacterEquipmentComponent _equipmentComponent;

        #endregion

        #region Locomotion Settings

        [Header("Player Locomotion")]
        [Header("Main Settings")]
        [Tooltip("Whether the character always faces the camera facing direction")]
        [SerializeField]
        private bool _alwaysStrafe = true;
        [Tooltip("Slowest movement speed of the player when set to a walk state or half press tick")]
        [SerializeField]
        private float _walkSpeed = 1.4f;
        [Tooltip("Default movement speed of the player")]
        [SerializeField]
        private float _runSpeed = 2.5f;
        [Tooltip("Top movement speed of the player")]
        [SerializeField]
        private float _sprintSpeed = 7f;
        [Tooltip("Damping factor for changing speed")]
        [SerializeField]
        private float _speedChangeDamping = 10f;
        [Tooltip("Rotation smoothing factor.")]
        [SerializeField]
        private float _rotationSmoothing = 10f;
        [Tooltip("Offset for camera rotation.")]
        [SerializeField]
        private float _cameraRotationOffset;

        #endregion

        #region Shuffle Settings

        [Header("Shuffles")]
        [Tooltip("Threshold for button hold duration.")]
        [SerializeField]
        private float _buttonHoldThreshold = 0.15f;
        [Tooltip("Direction of shuffling on the X-axis.")]
        [SerializeField]
        private float _shuffleDirectionX;
        [Tooltip("Direction of shuffling on the Z-axis.")]
        [SerializeField]
        private float _shuffleDirectionZ;

        #endregion

        #region Capsule Settings

        [Header("Capsule Values")]
        [Tooltip("Standing height of the player capsule.")]
        [SerializeField]
        private float _capsuleStandingHeight = 1.8f;
        [Tooltip("Standing center of the player capsule.")]
        [SerializeField]
        private float _capsuleStandingCentre = 0.93f;
        [Tooltip("Crouching height of the player capsule.")]
        [SerializeField]
        private float _capsuleCrouchingHeight = 1.2f;
        [Tooltip("Crouching center of the player capsule.")]
        [SerializeField]
        private float _capsuleCrouchingCentre = 0.6f;

        #endregion

        #region Strafing

        [Header("Player Strafing")]
        [Tooltip("Minimum threshold for forward strafing angle.")]
        [SerializeField]
        private float _forwardStrafeMinThreshold = -55.0f;
        [Tooltip("Maximum threshold for forward strafing angle.")]
        [SerializeField]
        private float _forwardStrafeMaxThreshold = 125.0f;
        [Tooltip("Current forward strafing value.")]
        [SerializeField]
        private float _forwardStrafe = 1f;

        #endregion

        #region Grounded Settings

        [Header("Grounded Angle")]
        [Tooltip("Position of the rear ray for grounded angle check.")]
        [SerializeField]
        private Transform _rearRayPos;
        [Tooltip("Position of the front ray for grounded angle check.")]
        [SerializeField]
        private Transform _frontRayPos;
        [Tooltip("Layer mask for checking ground.")]
        [SerializeField]
        private LayerMask _groundLayerMask;
        [Tooltip("Current incline angle.")]
        [SerializeField]
        private float _inclineAngle;
        [Tooltip("Useful for rough ground")]
        [SerializeField]
        private float _groundedOffset = -0.14f;

        #endregion

        #region In-Air Settings

        [Header("Player In-Air")]
        [Tooltip("Force applied when the player jumps.")]
        [SerializeField]
        private float _jumpForce = 10f;
        [Tooltip("Multiplier for gravity when in the air.")]
        [SerializeField]
        private float _gravityMultiplier = 2f;
        [Tooltip("Duration of falling.")]
        [SerializeField]
        private float _fallingDuration;

        #endregion

        #region Head Look Settings

        [Header("Player Head Look")]
        [Tooltip("Flag indicating if head turning is enabled.")]
        [SerializeField]
        private bool _enableHeadTurn = true;
        [Tooltip("Delay for head turning.")]
        [SerializeField]
        private float _headLookDelay;
        [Tooltip("X-axis value for head turning.")]
        [SerializeField]
        private float _headLookX;
        [Tooltip("Y-axis value for head turning.")]
        [SerializeField]
        private float _headLookY;
        [Tooltip("Curve for X-axis head turning.")]
        [SerializeField]
        private AnimationCurve _headLookXCurve;

        #endregion

        #region Body Look Settings

        [Header("Player Body Look")]
        [Tooltip("Flag indicating if body turning is enabled.")]
        [SerializeField]
        private bool _enableBodyTurn = true;
        [Tooltip("Delay for body turning.")]
        [SerializeField]
        private float _bodyLookDelay;
        [Tooltip("X-axis value for body turning.")]
        [SerializeField]
        private float _bodyLookX;
        [Tooltip("Y-axis value for body turning.")]
        [SerializeField]
        private float _bodyLookY;
        [Tooltip("Curve for X-axis body turning.")]
        [SerializeField]
        private AnimationCurve _bodyLookXCurve;

        #endregion

        #region Lean Settings

        [Header("Player Lean")]
        [Tooltip("Flag indicating if leaning is enabled.")]
        [SerializeField]
        private bool _enableLean = true;
        [Tooltip("Delay for leaning.")]
        [SerializeField]
        private float _leanDelay;
        [Tooltip("Current value for leaning.")]
        [SerializeField]
        private float _leanValue;
        [Tooltip("Curve for leaning.")]
        [SerializeField]
        private AnimationCurve _leanCurve;
        [Tooltip("Delay for head leaning looks.")]
        [SerializeField]
        private float _leansHeadLooksDelay;
        [Tooltip("Flag indicating if an animation clip has ended.")]
        [SerializeField]
        private bool _animationClipEnd;

        #endregion

        #endregion

        #region Runtime Properties

        private readonly List<GameObject> _currentTargetCandidates = new List<GameObject>();
        private AnimationState _currentState = AnimationState.Base;
        private bool _cannotStandUp;
        private bool _crouchKeyPressed;
        private bool _isAiming;
        private bool _isCrouching;
        private bool _isGrounded = true;
        private bool _isLockedOn;
        private bool _isSliding;
        private bool _isSprinting;
        private bool _isStarting;
        private bool _isStopped = true;
        private bool _isStrafing;
        private bool _isTurningInPlace;
        private bool _isWalking;
        private bool _movementInputHeld;
        private bool _movementInputPressed;
        private bool _movementInputTapped;
        private float _currentMaxSpeed;
        private float _locomotionStartDirection;
        private float _locomotionStartTimer;
        private float _lookingAngle;
        private float _newDirectionDifferenceAngle;
        private float _speed2D;
        private float _strafeAngle;
        private float _strafeDirectionX;
        private float _strafeDirectionZ;
        private GameObject _currentLockOnTarget;
        private GaitState _currentGait;
        private Transform _targetLockOnPos;
        private Vector3 _currentRotation = new Vector3(0f, 0f, 0f);
        private Vector3 _moveDirection;
        private Vector3 _previousRotation;
        private Vector3 _velocity;

        #endregion

        #region Base State Variables

        private const float _ANIMATION_DAMP_TIME = 5f;
        private const float _STRAFE_DIRECTION_DAMP_TIME = 20f;
        private float _targetMaxSpeed;
        private float _fallStartTime;
        private float _rotationRate;
        private float _initialLeanValue;
        private float _initialTurnValue;
        private Vector3 _cameraForward;
        private Vector3 _targetVelocity;

        #endregion

        #region Animation Controller

        #region Start

        private void Start()
        {
            _equipmentComponent = GetComponent<CharacterEquipmentComponent>();
            _targetLockOnPos = transform.Find("TargetLockOnPos");

            _inputReader.onLockOnToggled += ToggleLockOn;
            _inputReader.onWalkToggled += ToggleWalk;
            _inputReader.onSprintActivated += ActivateSprint;
            _inputReader.onSprintDeactivated += DeactivateSprint;
            _inputReader.onCrouchActivated += ActivateCrouch;
            _inputReader.onCrouchDeactivated += DeactivateCrouch;
            _inputReader.onAimActivated += ActivateAim;
            _inputReader.onAimDeactivated += DeactivateAim;
            _inputReader.onFirePerformed += FireWeapon;
            _inputReader.onReloadPerformed += ReloadWeapon;
            _inputReader.onWeaponSwitchPerformed += SwitchWeapon;

            _isStrafing = _alwaysStrafe;

            SwitchState(AnimationState.Locomotion);
        }

        #endregion

        #region Aim and Lock-on

        private void ActivateAim()
        {
            _isAiming = true;
            _isStrafing = !_isSprinting;
        }

        private void DeactivateAim()
        {
            _isAiming = false;
            _isStrafing = !_isSprinting && (_alwaysStrafe || _isLockedOn);
        }

        public void AddTargetCandidate(GameObject newTarget)
        {
            if (newTarget != null)
            {
                _currentTargetCandidates.Add(newTarget);
            }
        }

        public void RemoveTarget(GameObject targetToRemove)
        {
            if (_currentTargetCandidates.Contains(targetToRemove))
            {
                _currentTargetCandidates.Remove(targetToRemove);
            }
        }

        private void ToggleLockOn()
        {
            EnableLockOn(!_isLockedOn);
        }

        private void EnableLockOn(bool enable)
        {
            _isLockedOn = enable;
            _isStrafing = false;
            _isStrafing = enable ? !_isSprinting : _alwaysStrafe || _isAiming;
            _cameraController.LockOn(enable, _targetLockOnPos);

            if (enable && _currentLockOnTarget != null)
            {
                _currentLockOnTarget.GetComponent<SampleObjectLockOn>().Highlight(true, true);
            }
        }

        #endregion

        #region Walking State

        private void ToggleWalk()
        {
            EnableWalk(!_isWalking);
        }

        private void EnableWalk(bool enable)
        {
            _isWalking = enable && _isGrounded && !_isSprinting;
        }

        #endregion

        #region Sprinting State

        private void ActivateSprint()
        {
            if (!_isCrouching)
            {
                EnableWalk(false);
                _isSprinting = true;
                _isStrafing = false;
            }
        }

        private void DeactivateSprint()
        {
            _isSprinting = false;
            if (_alwaysStrafe || _isAiming || _isLockedOn)
            {
                _isStrafing = true;
            }
        }

        #endregion

        #region Crouching State

        private void ActivateCrouch()
        {
            _crouchKeyPressed = true;
            if (_isGrounded)
            {
                CapsuleCrouchingSize(true);
                DeactivateSprint();
                _isCrouching = true;
            }
        }

        private void DeactivateCrouch()
        {
            _crouchKeyPressed = false;
            if (!_cannotStandUp && !_isSliding)
            {
                CapsuleCrouchingSize(false);
                _isCrouching = false;
            }
        }

        public void ActivateSliding()
        {
            _isSliding = true;
        }

        public void DeactivateSliding()
        {
            _isSliding = false;
        }

        private void CapsuleCrouchingSize(bool crouching)
        {
            if (crouching)
            {
                _controller.center = new Vector3(0f, _capsuleCrouchingCentre, 0f);
                _controller.height = _capsuleCrouchingHeight;
            }
            else
            {
                _controller.center = new Vector3(0f, _capsuleStandingCentre, 0f);
                _controller.height = _capsuleStandingHeight;
            }
        }

        #endregion

        #region Weapon Actions

        private void FireWeapon()
        {
            if (_equipmentComponent != null && 
                _currentState != AnimationState.Jump &&
                _currentState != AnimationState.Fall &&
                _equipmentComponent.currentEquippedWeapon is IRangedWeapon rangedWeapon)
            {
                rangedWeapon.StartFire();
                _animator.SetBool(_isFiringHash, true);
                SwitchState(AnimationState.WeaponAction);
            }
        }

        private void ReloadWeapon()
        {
            if (_equipmentComponent != null &&
                _currentState != AnimationState.Jump &&
                _currentState != AnimationState.Fall &&
                _equipmentComponent.currentEquippedWeapon is IRangedWeapon rangedWeapon)
            {
                rangedWeapon.StartReload();
                _animator.SetBool(_isReloadingHash, true);
                SwitchState(AnimationState.WeaponAction);
            }
        }

        private void SwitchWeapon(int weaponIndex)
        {
            _equipmentComponent.EquipItemInSlot((EquipmentSlots)weaponIndex);
        }

        #endregion

        #endregion

        #region Shared State

        #region State Change

        private void SwitchState(AnimationState newState)
        {
            ExitCurrentState();
            EnterState(newState);
        }

        private void EnterState(AnimationState stateToEnter)
        {
            _currentState = stateToEnter;
            switch (_currentState)
            {
                case AnimationState.Base:
                    EnterBaseState();
                    break;
                case AnimationState.Locomotion:
                    EnterLocomotionState();
                    break;
                case AnimationState.Jump:
                    EnterJumpState();
                    break;
                case AnimationState.Fall:
                    EnterFallState();
                    break;
                case AnimationState.Crouch:
                    EnterCrouchState();
                    break;
                case AnimationState.WeaponAction:
                    EnterWeaponActionState();
                    break;
            }
        }

        private void ExitCurrentState()
        {
            switch (_currentState)
            {
                case AnimationState.Locomotion:
                    ExitLocomotionState();
                    break;
                case AnimationState.Jump:
                    ExitJumpState();
                    break;
                case AnimationState.Crouch:
                    ExitCrouchState();
                    break;
                case AnimationState.WeaponAction:
                    ExitWeaponActionState();
                    break;
            }
        }

        #endregion

        #region Updates

        private void Update()
        {
            switch (_currentState)
            {
                case AnimationState.Locomotion:
                    UpdateLocomotionState();
                    break;
                case AnimationState.Jump:
                    UpdateJumpState();
                    break;
                case AnimationState.Fall:
                    UpdateFallState();
                    break;
                case AnimationState.Crouch:
                    UpdateCrouchState();
                    break;
                case AnimationState.WeaponAction:
                    UpdateWeaponActionState();
                    break;
            }
        }

        private void UpdateAnimatorController()
        {
            _animator.SetFloat(_leanValueHash, _leanValue);
            _animator.SetFloat(_headLookXHash, _headLookX);
            _animator.SetFloat(_headLookYHash, _headLookY);
            _animator.SetFloat(_bodyLookXHash, _bodyLookX);
            _animator.SetFloat(_bodyLookYHash, _bodyLookY);

            _animator.SetFloat(_isStrafingHash, _cameraController._controlType == SampleCameraController.ControlType.TopDown || _isStrafing ? 1.0f : 0.0f);

            _animator.SetFloat(_inclineAngleHash, _inclineAngle);

            _animator.SetFloat(_moveSpeedHash, _speed2D);
            _animator.SetInteger(_currentGaitHash, (int)_currentGait);

            _animator.SetFloat(_strafeDirectionXHash, _strafeDirectionX);
            _animator.SetFloat(_strafeDirectionZHash, _strafeDirectionZ);
            _animator.SetFloat(_forwardStrafeHash, _forwardStrafe);
            _animator.SetFloat(_cameraRotationOffsetHash, _cameraRotationOffset);

            _animator.SetBool(_movementInputHeldHash, _movementInputHeld);
            _animator.SetBool(_movementInputPressedHash, _movementInputPressed);
            _animator.SetBool(_movementInputTappedHash, _movementInputTapped);
            _animator.SetFloat(_shuffleDirectionXHash, _shuffleDirectionX);
            _animator.SetFloat(_shuffleDirectionZHash, _shuffleDirectionZ);

            _animator.SetBool(_isTurningInPlaceHash, _isTurningInPlace);
            _animator.SetBool(_isCrouchingHash, _isCrouching);

            _animator.SetFloat(_fallingDurationHash, _fallingDuration);
            _animator.SetBool(_isGroundedHash, _isGrounded);

            _animator.SetBool(_isWalkingHash, _isWalking);
            _animator.SetBool(_isStoppedHash, _isStopped);

            _animator.SetFloat(_locomotionStartDirectionHash, _locomotionStartDirection);
        }

        #endregion

        #endregion

        #region Base State

        #region Setup

        private void EnterBaseState()
        {
            _previousRotation = transform.forward;
        }

        private void CalculateInput()
        {
            if (_inputReader._movementInputDetected)
            {
                if (_inputReader._movementInputDuration == 0)
                {
                    _movementInputTapped = true;
                }
                else if (_inputReader._movementInputDuration > 0 && _inputReader._movementInputDuration < _buttonHoldThreshold)
                {
                    _movementInputTapped = false;
                    _movementInputPressed = true;
                    _movementInputHeld = false;
                }
                else
                {
                    _movementInputTapped = false;
                    _movementInputPressed = false;
                    _movementInputHeld = true;
                }

                _inputReader._movementInputDuration += Time.deltaTime;
            }
            else
            {
                _inputReader._movementInputDuration = 0;
                _movementInputTapped = false;
                _movementInputPressed = false;
                _movementInputHeld = false;
            }

            if (_cameraController._controlType == SampleCameraController.ControlType.ThirdPerson)
            {
                _moveDirection = (_cameraController.GetCameraForwardZeroedYNormalised() * _inputReader._moveComposite.y)
                    + (_cameraController.GetCameraRightZeroedYNormalised() * _inputReader._moveComposite.x);
            }
            else
            {
                _moveDirection = new Vector3(_inputReader._moveComposite.x, 0, _inputReader._moveComposite.y).normalized;
            }
        }

        #endregion

        #region Movement

        private void Move()
        {
            _controller.Move(_velocity * Time.deltaTime);

            if (_isLockedOn)
            {
                if (_currentLockOnTarget != null)
                {
                    _targetLockOnPos.position = _currentLockOnTarget.transform.position;
                }
            }
        }

        private void ApplyGravity()
        {
            if (_velocity.y > Physics.gravity.y)
            {
                _velocity.y += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
            }
        }

        private Vector3 GetCursorWorldPosition()
        {
            Camera activeCamera = _cameraController._controlType == SampleCameraController.ControlType.ThirdPerson ?
                _thirdPersonCamera : _topDownCamera;
            if (activeCamera == null)
            {
                Debug.LogError("Active camera is null. Please assign cameras in the inspector.");
                return Vector3.zero;
            }

            Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            if (groundPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
            return Vector3.zero;
        }

        private void CalculateMoveDirection()
        {
            CalculateInput();

            if (!_isGrounded)
            {
                _targetMaxSpeed = _currentMaxSpeed;
            }
            else if (_isCrouching)
            {
                _targetMaxSpeed = _walkSpeed;
            }
            else if (_isSprinting)
            {
                _targetMaxSpeed = _sprintSpeed;
            }
            else if (_isWalking)
            {
                _targetMaxSpeed = _walkSpeed;
            }
            else
            {
                _targetMaxSpeed = _runSpeed;
            }

            _currentMaxSpeed = Mathf.Lerp(_currentMaxSpeed, _targetMaxSpeed, _ANIMATION_DAMP_TIME * Time.deltaTime);

            _targetVelocity.x = _moveDirection.x * _currentMaxSpeed;
            _targetVelocity.z = _moveDirection.z * _currentMaxSpeed;

            _velocity.z = Mathf.Lerp(_velocity.z, _targetVelocity.z, _speedChangeDamping * Time.deltaTime);
            _velocity.x = Mathf.Lerp(_velocity.x, _targetVelocity.x, _speedChangeDamping * Time.deltaTime);

            _speed2D = new Vector3(_velocity.x, 0f, _velocity.z).magnitude;
            _speed2D = Mathf.Round(_speed2D * 1000f) / 1000f;

            Vector3 playerForwardVector = transform.forward;

            _newDirectionDifferenceAngle = playerForwardVector != _moveDirection
                ? Vector3.SignedAngle(playerForwardVector, _moveDirection, Vector3.up)
                : 0f;

            CalculateGait();
        }

        private void CalculateGait()
        {
            float runThreshold = (_walkSpeed + _runSpeed) / 2;
            float sprintThreshold = (_runSpeed + _sprintSpeed) / 2;

            if (_speed2D < 0.01)
            {
                _currentGait = GaitState.Idle;
            }
            else if (_speed2D < runThreshold)
            {
                _currentGait = GaitState.Walk;
            }
            else if (_speed2D < sprintThreshold)
            {
                _currentGait = GaitState.Run;
            }
            else
            {
                _currentGait = GaitState.Sprint;
            }
        }

        private void FaceMoveDirection()
        {
            if (_cameraController._controlType == SampleCameraController.ControlType.ThirdPerson)
            {
                Vector3 characterForward = new Vector3(transform.forward.x, 0f, transform.forward.z).normalized;
                Vector3 characterRight = new Vector3(transform.right.x, 0f, transform.right.z).normalized;
                Vector3 directionForward = new Vector3(_moveDirection.x, 0f, _moveDirection.z).normalized;

                _cameraForward = _cameraController.GetCameraForwardZeroedYNormalised();
                Quaternion strafingTargetRotation = Quaternion.LookRotation(_cameraForward);

                _strafeAngle = characterForward != directionForward ? Vector3.SignedAngle(characterForward, directionForward, Vector3.up) : 0f;

                _isTurningInPlace = false;

                if (_isStrafing)
                {
                    if (_moveDirection.magnitude > 0.01)
                    {
                        if (_cameraForward != Vector3.zero)
                        {
                            _shuffleDirectionZ = Vector3.Dot(characterForward, directionForward);
                            _shuffleDirectionX = Vector3.Dot(characterRight, directionForward);

                            UpdateStrafeDirection(
                                Vector3.Dot(characterForward, directionForward),
                                Vector3.Dot(characterRight, directionForward)
                            );
                            _cameraRotationOffset = Mathf.Lerp(_cameraRotationOffset, 0f, _rotationSmoothing * Time.deltaTime);

                            float targetValue = _strafeAngle > _forwardStrafeMinThreshold && _strafeAngle < _forwardStrafeMaxThreshold ? 1f : 0f;

                            if (Mathf.Abs(_forwardStrafe - targetValue) <= 0.001f)
                            {
                                _forwardStrafe = targetValue;
                            }
                            else
                            {
                                float t = Mathf.Clamp01(_STRAFE_DIRECTION_DAMP_TIME * Time.deltaTime);
                                _forwardStrafe = Mathf.SmoothStep(_forwardStrafe, targetValue, t);
                            }
                        }

                        transform.rotation = Quaternion.Slerp(transform.rotation, strafingTargetRotation, _rotationSmoothing * Time.deltaTime);
                    }
                    else
                    {
                        UpdateStrafeDirection(1f, 0f);

                        float t = 20 * Time.deltaTime;
                        float newOffset = 0f;

                        if (characterForward != _cameraForward)
                        {
                            newOffset = Vector3.SignedAngle(characterForward, _cameraForward, Vector3.up);
                        }

                        _cameraRotationOffset = Mathf.Lerp(_cameraRotationOffset, newOffset, t);

                        if (Mathf.Abs(_cameraRotationOffset) > 10)
                        {
                            _isTurningInPlace = true;
                        }
                    }
                }
                else
                {
                    UpdateStrafeDirection(1f, 0f);
                    _cameraRotationOffset = Mathf.Lerp(_cameraRotationOffset, 0f, _rotationSmoothing * Time.deltaTime);

                    _shuffleDirectionZ = 1;
                    _shuffleDirectionX = 0;

                    Vector3 faceDirection = new Vector3(_velocity.x, 0f, _velocity.z);

                    if (faceDirection == Vector3.zero)
                    {
                        return;
                    }

                    transform.rotation = Quaternion.Slerp(
                        transform.rotation,
                        Quaternion.LookRotation(faceDirection),
                        _rotationSmoothing * Time.deltaTime
                    );
                }
            }
            else if (_cameraController._controlType == SampleCameraController.ControlType.TopDown)
            {
                Vector3 cursorPosition = GetCursorWorldPosition();
                Vector3 directionToCursor = cursorPosition - transform.position;
                directionToCursor.y = 0;
                if (directionToCursor != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(directionToCursor);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSmoothing * Time.deltaTime);
                }

                Vector3 localMoveDirection = transform.InverseTransformDirection(_moveDirection);
                UpdateStrafeDirection(localMoveDirection.z, localMoveDirection.x);
            }
        }

        private void CheckIfStopped()
        {
            _isStopped = _moveDirection.magnitude == 0 && _speed2D < .5;
        }

        private void CheckIfStarting()
        {
            _locomotionStartTimer = VariableOverrideDelayTimer(_locomotionStartTimer);

            bool isStartingCheck = false;

            if (_locomotionStartTimer <= 0.0f)
            {
                if (_moveDirection.magnitude > 0.01 && _speed2D < 1 && !_isStrafing)
                {
                    isStartingCheck = true;
                }

                if (isStartingCheck)
                {
                    if (!_isStarting)
                    {
                        _locomotionStartDirection = _newDirectionDifferenceAngle;
                        _animator.SetFloat(_locomotionStartDirectionHash, _locomotionStartDirection);
                    }

                    float delayTime = 0.2f;
                    _leanDelay = delayTime;
                    _headLookDelay = delayTime;
                    _bodyLookDelay = delayTime;

                    _locomotionStartTimer = delayTime;
                }
            }
            else
            {
                isStartingCheck = true;
            }

            _isStarting = isStartingCheck;
            _animator.SetBool(_isStartingHash, _isStarting);
        }

        private void UpdateStrafeDirection(float TargetZ, float TargetX)
        {
            _strafeDirectionZ = Mathf.Lerp(_strafeDirectionZ, TargetZ, _ANIMATION_DAMP_TIME * Time.deltaTime);
            _strafeDirectionX = Mathf.Lerp(_strafeDirectionX, TargetX, _ANIMATION_DAMP_TIME * Time.deltaTime);
            _strafeDirectionZ = Mathf.Round(_strafeDirectionZ * 1000f) / 1000f;
            _strafeDirectionX = Mathf.Round(_strafeDirectionX * 1000f) / 1000f;
        }

        #endregion

        #region Ground Checks

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(
                _controller.transform.position.x,
                _controller.transform.position.y - _groundedOffset,
                _controller.transform.position.z
            );
            _isGrounded = Physics.CheckSphere(spherePosition, _controller.radius, _groundLayerMask, QueryTriggerInteraction.Ignore);

            if (_isGrounded)
            {
                GroundInclineCheck();
            }
        }

        private void GroundInclineCheck()
        {
            float rayDistance = Mathf.Infinity;
            _rearRayPos.rotation = Quaternion.Euler(transform.rotation.x, 0, 0);
            _frontRayPos.rotation = Quaternion.Euler(transform.rotation.x, 0, 0);

            Physics.Raycast(_rearRayPos.position, _rearRayPos.TransformDirection(-Vector3.up), out RaycastHit rearHit, rayDistance, _groundLayerMask);
            Physics.Raycast(
                _frontRayPos.position,
                _frontRayPos.TransformDirection(-Vector3.up),
                out RaycastHit frontHit,
                rayDistance,
                _groundLayerMask
            );

            Vector3 hitDifference = frontHit.point - rearHit.point;
            float xPlaneLength = new Vector2(hitDifference.x, hitDifference.z).magnitude;

            _inclineAngle = Mathf.Lerp(_inclineAngle, Mathf.Atan2(hitDifference.y, xPlaneLength) * Mathf.Rad2Deg, 20f * Time.deltaTime);
        }

        private void CeilingHeightCheck()
        {
            float rayDistance = Mathf.Infinity;
            float minimumStandingHeight = _capsuleStandingHeight - _frontRayPos.localPosition.y;

            Vector3 midpoint = new Vector3(transform.position.x, transform.position.y + _frontRayPos.localPosition.y, transform.position.z);
            if (Physics.Raycast(midpoint, transform.TransformDirection(Vector3.up), out RaycastHit ceilingHit, rayDistance, _groundLayerMask))
            {
                _cannotStandUp = ceilingHit.distance < minimumStandingHeight;
            }
            else
            {
                _cannotStandUp = false;
            }
        }

        #endregion

        #region Falling

        private void ResetFallingDuration()
        {
            _fallStartTime = Time.time;
            _fallingDuration = 0f;
        }

        private void UpdateFallingDuration()
        {
            _fallingDuration = Time.time - _fallStartTime;
        }

        #endregion

        #region Checks

        private void CheckEnableTurns()
        {
            _headLookDelay = VariableOverrideDelayTimer(_headLookDelay);
            _enableHeadTurn = _headLookDelay == 0.0f && !_isStarting;
            _bodyLookDelay = VariableOverrideDelayTimer(_bodyLookDelay);
            _enableBodyTurn = _bodyLookDelay == 0.0f && !(_isStarting || _isTurningInPlace);
        }

        private void CheckEnableLean()
        {
            _leanDelay = VariableOverrideDelayTimer(_leanDelay);
            _enableLean = _leanDelay == 0.0f && !(_isStarting || _isTurningInPlace);
        }

        #endregion

        #region Lean and Offsets

        private void CalculateRotationalAdditives(bool leansActivated, bool headLookActivated, bool bodyLookActivated)
        {
            if (headLookActivated || leansActivated || bodyLookActivated)
            {
                _currentRotation = transform.forward;

                _rotationRate = _currentRotation != _previousRotation
                    ? Vector3.SignedAngle(_currentRotation, _previousRotation, Vector3.up) / Time.deltaTime * -1f
                    : 0f;
            }

            _initialLeanValue = leansActivated ? _rotationRate : 0f;

            float leanSmoothness = 5;
            float maxLeanRotationRate = 275.0f;

            float referenceValue = _speed2D / _sprintSpeed;
            _leanValue = CalculateSmoothedValue(
                _leanValue,
                _initialLeanValue,
                maxLeanRotationRate,
                leanSmoothness,
                _leanCurve,
                referenceValue,
                true
            );

            float headTurnSmoothness = 5f;

            if (headLookActivated && _isTurningInPlace)
            {
                _initialTurnValue = _cameraRotationOffset;
                _headLookX = Mathf.Lerp(_headLookX, _initialTurnValue / 200, 5f * Time.deltaTime);
            }
            else
            {
                _initialTurnValue = headLookActivated ? _rotationRate : 0f;
                _headLookX = CalculateSmoothedValue(
                    _headLookX,
                    _initialTurnValue,
                    maxLeanRotationRate,
                    headTurnSmoothness,
                    _headLookXCurve,
                    _headLookX,
                    false
                );
            }

            float bodyTurnSmoothness = 5f;

            _initialTurnValue = bodyLookActivated ? _rotationRate : 0f;

            _bodyLookX = CalculateSmoothedValue(
                _bodyLookX,
                _initialTurnValue,
                maxLeanRotationRate,
                bodyTurnSmoothness,
                _bodyLookXCurve,
                _bodyLookX,
                false
            );

            float cameraTilt = _cameraController.GetCameraTiltX();
            cameraTilt = (cameraTilt > 180f ? cameraTilt - 360f : cameraTilt) / -180;
            cameraTilt = Mathf.Clamp(cameraTilt, -0.1f, 1.0f);
            _headLookY = cameraTilt;
            _bodyLookY = cameraTilt;

            _previousRotation = _currentRotation;
        }

        private float CalculateSmoothedValue(
            float mainVariable,
            float newValue,
            float maxRateChange,
            float smoothness,
            AnimationCurve referenceCurve,
            float referenceValue,
            bool isMultiplier
        )
        {
            float changeVariable = newValue / maxRateChange;

            changeVariable = Mathf.Clamp(changeVariable, -1.0f, 1.0f);

            if (isMultiplier)
            {
                float multiplier = referenceCurve.Evaluate(referenceValue);
                changeVariable *= multiplier;
            }
            else
            {
                changeVariable = referenceCurve.Evaluate(changeVariable);
            }

            if (!changeVariable.Equals(mainVariable))
            {
                changeVariable = Mathf.Lerp(mainVariable, changeVariable, smoothness * Time.deltaTime);
            }

            return changeVariable;
        }

        private float VariableOverrideDelayTimer(float timeVariable)
        {
            if (timeVariable > 0.0f)
            {
                timeVariable -= Time.deltaTime;
                timeVariable = Mathf.Clamp(timeVariable, 0.0f, 1.0f);
            }
            else
            {
                timeVariable = 0.0f;
            }

            return timeVariable;
        }

        #endregion

        #region Lock-on System

        private void UpdateBestTarget()
        {
            GameObject newBestTarget;

            if (_currentTargetCandidates.Count == 0)
            {
                newBestTarget = null;
            }
            else if (_currentTargetCandidates.Count == 1)
            {
                newBestTarget = _currentTargetCandidates[0];
            }
            else
            {
                newBestTarget = null;
                float bestTargetScore = 0f;

                foreach (GameObject target in _currentTargetCandidates)
                {
                    target.GetComponent<SampleObjectLockOn>().Highlight(false, false);

                    float distance = Vector3.Distance(transform.position, target.transform.position);
                    float distanceScore = 1 / distance * 100;

                    Vector3 targetDirection = target.transform.position - _cameraController.GetCameraPosition();
                    float angleInView = Vector3.Dot(targetDirection.normalized, _cameraController.GetCameraForward());
                    float angleScore = angleInView * 40;

                    float totalScore = distanceScore + angleScore;

                    if (totalScore > bestTargetScore)
                    {
                        bestTargetScore = totalScore;
                        newBestTarget = target;
                    }
                }
            }

            if (!_isLockedOn)
            {
                _currentLockOnTarget = newBestTarget;

                if (_currentLockOnTarget != null)
                {
                    _currentLockOnTarget.GetComponent<SampleObjectLockOn>().Highlight(true, false);
                }
            }
            else
            {
                if (_currentTargetCandidates.Contains(_currentLockOnTarget))
                {
                    _currentLockOnTarget.GetComponent<SampleObjectLockOn>().Highlight(true, true);
                }
                else
                {
                    _currentLockOnTarget = newBestTarget;
                    EnableLockOn(false);
                }
            }
        }

        #endregion

        #endregion

        #region Locomotion State

        private void EnterLocomotionState()
        {
            _inputReader.onJumpPerformed += LocomotionToJumpState;
        }

        private void UpdateLocomotionState()
        {
            UpdateBestTarget();
            GroundedCheck();

            if (!_isGrounded)
            {
                SwitchState(AnimationState.Fall);
            }

            if (_isCrouching)
            {
                SwitchState(AnimationState.Crouch);
            }

            CheckEnableTurns();
            CheckEnableLean();
            CalculateRotationalAdditives(_enableLean, _enableHeadTurn, _enableBodyTurn);

            CalculateMoveDirection();
            CheckIfStarting();
            CheckIfStopped();
            FaceMoveDirection();
            Move();
            UpdateAnimatorController();
        }

        private void ExitLocomotionState()
        {
            _inputReader.onJumpPerformed -= LocomotionToJumpState;
        }

        private void LocomotionToJumpState()
        {
            SwitchState(AnimationState.Jump);
        }

        #endregion

        #region Jump State

        private void EnterJumpState()
        {
            _animator.SetBool(_isJumpingAnimHash, true);

            _isSliding = false;

            _velocity = new Vector3(_velocity.x, _jumpForce, _velocity.z);
        }

        private void UpdateJumpState()
        {
            UpdateBestTarget();
            ApplyGravity();

            if (_velocity.y <= 0f)
            {
                _animator.SetBool(_isJumpingAnimHash, false);
                SwitchState(AnimationState.Fall);
            }

            GroundedCheck();

            CalculateRotationalAdditives(false, _enableHeadTurn, _enableBodyTurn);
            CalculateMoveDirection();
            FaceMoveDirection();
            Move();
            UpdateAnimatorController();
        }

        private void ExitJumpState()
        {
            _animator.SetBool(_isJumpingAnimHash, false);
        }

        #endregion

        #region Fall State

        private void EnterFallState()
        {
            ResetFallingDuration();
            _velocity.y = 0f;

            DeactivateCrouch();
            _isSliding = false;
        }

        private void UpdateFallState()
        {
            UpdateBestTarget();
            GroundedCheck();

            CalculateRotationalAdditives(false, _enableHeadTurn, _enableBodyTurn);

            CalculateMoveDirection();
            FaceMoveDirection();

            ApplyGravity();
            Move();
            UpdateAnimatorController();

            if (_controller.isGrounded)
            {
                SwitchState(AnimationState.Locomotion);
            }

            UpdateFallingDuration();
        }

        #endregion

        #region Crouch State

        private void EnterCrouchState()
        {
            _inputReader.onJumpPerformed += CrouchToJumpState;
        }

        private void UpdateCrouchState()
        {
            UpdateBestTarget();

            GroundedCheck();
            if (!_isGrounded)
            {
                DeactivateCrouch();
                CapsuleCrouchingSize(false);
                SwitchState(AnimationState.Fall);
            }

            CeilingHeightCheck();

            if (!_crouchKeyPressed && !_cannotStandUp)
            {
                DeactivateCrouch();
                SwitchToLocomotionState();
            }

            if (!_isCrouching)
            {
                CapsuleCrouchingSize(false);
                SwitchToLocomotionState();
            }

            CheckEnableTurns();
            CheckEnableLean();

            CalculateRotationalAdditives(false, _enableHeadTurn, false);

            CalculateMoveDirection();
            CheckIfStarting();
            CheckIfStopped();

            FaceMoveDirection();
            Move();
            UpdateAnimatorController();
        }

        private void ExitCrouchState()
        {
            _inputReader.onJumpPerformed -= CrouchToJumpState;
        }

        private void CrouchToJumpState()
        {
            if (!_cannotStandUp)
            {
                DeactivateCrouch();
                SwitchState(AnimationState.Jump);
            }
        }

        private void SwitchToLocomotionState()
        {
            DeactivateCrouch();
            SwitchState(AnimationState.Locomotion);
        }

        #endregion

        #region Weapon State

        private void EnterWeaponActionState()
        {
            // nothing to do yet
        }

        private void UpdateWeaponActionState()
        {
            UpdateBestTarget();
            GroundedCheck();

            if (!_isGrounded)
            {
                _animator.SetBool(_isFiringHash, false);
                _animator.SetBool(_isReloadingHash, false);
                SwitchState(AnimationState.Fall);
                return;
            }

            if (_equipmentComponent.currentEquippedWeapon is IRangedWeapon rangedWeapon)
            {
                if (!rangedWeapon.IsReloading())
                    _animator.SetBool(_isReloadingHash, false);

                if (!Input.GetButton("Fire1") && !rangedWeapon.IsReloading())
                {
                    _animator.SetBool(_isFiringHash, false);
                    rangedWeapon.StopFire();
                    SwitchState(AnimationState.Locomotion);
                }
            }

            CalculateMoveDirection();
            FaceMoveDirection();
            Move();
            UpdateAnimatorController();
        }

        private void ExitWeaponActionState()
        {
            _animator.SetBool(_isFiringHash, false);
            _animator.SetBool(_isReloadingHash, false);
        }

        #endregion

        #region Getters

        public Animator GetAnimator() => _animator;

        #endregion
    }
}
