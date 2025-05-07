using Synty.AnimationBaseLocomotion.Samples.InputSystem;
using UnityEngine;

namespace Synty.AnimationBaseLocomotion.Samples
{
    public class SampleCameraController : MonoBehaviour
    {
        private const int _LAG_DELTA_TIME_ADJUSTMENT = 20;

        public enum ControlType
        {
            ThirdPerson,
            TopDown
        }

        [Tooltip("The character game object")]
        [SerializeField]
        private GameObject _syntyCharacter;
        [Tooltip("Main camera used for player perspective")]
        [SerializeField]
        private Camera _thirdPersonCamera;
        [SerializeField]
        private Camera _topDownCamera;

        [SerializeField]
        private Transform _playerTarget;
        [SerializeField]
        private Transform _lockOnTarget;

        [SerializeField]
        private bool _invertCamera;
        [SerializeField]
        private bool _hideCursor;
        [SerializeField]
        private bool _isLockedOn;
        [SerializeField]
        private float _mouseSensitivity = 5f;
        [SerializeField]
        private float _cameraDistance = 5f;
        [SerializeField]
        private float _cameraHeightOffset = 10f; // Увеличена высота для TopDown
        [SerializeField]
        private float _cameraHorizontalOffset;
        [SerializeField]
        private float _cameraTiltOffset;
        [SerializeField]
        private Vector2 _cameraTiltBounds = new Vector2(-10f, 45f);
        [SerializeField]
        private float _positionalCameraLagThirdPerson = 1f;
        [SerializeField]
        private float _positionalCameraLagTopDown = 10f;
        [SerializeField]
        private float _rotationalCameraLag = 1f;
        [SerializeField]
        public ControlType _controlType = ControlType.ThirdPerson;

        private float _cameraInversion;
        private InputReader _inputReader;
        private float _lastAngleX;
        private float _lastAngleY;
        private Vector3 _lastPosition;
        private float _newAngleX;
        private float _newAngleY;
        private Vector3 _newPosition;
        private float _rotationX;
        private float _rotationY;
        private Transform _syntyCamera;
        private Vector3 _topDownOffset;
        private Quaternion _topDownRotation;

        private void Start()
        {
            _syntyCamera = gameObject.transform.GetChild(0);
            _inputReader = _syntyCharacter.GetComponent<InputReader>();
            _playerTarget = _syntyCharacter.transform.Find("SyntyPlayer_LookAt");
            _lockOnTarget = _syntyCharacter.transform.Find("TargetLockOnPos");

            _topDownOffset = _topDownCamera.transform.position - _playerTarget.position;
            _topDownRotation = _topDownCamera.transform.rotation;

            if (_hideCursor)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            _cameraInversion = _invertCamera ? 1 : -1;
            transform.position = _playerTarget.position;
            transform.rotation = _playerTarget.rotation;
            _lastPosition = transform.position;

            _syntyCamera.localPosition = new Vector3(_cameraHorizontalOffset, _cameraHeightOffset, _cameraDistance * -1);
            _syntyCamera.localEulerAngles = new Vector3(_cameraTiltOffset, 0f, 0f);

            SetControlType(_controlType);
        }

        private void Update()
        {
            if (_controlType == ControlType.ThirdPerson)
            {
                UpdateThirdPersonCamera();
            }
            else if (_controlType == ControlType.TopDown)
            {
                UpdateTopDownCamera();
            }
        }

        private void UpdateThirdPersonCamera()
        {
            float positionalFollowSpeed = 1 / (_positionalCameraLagThirdPerson / _LAG_DELTA_TIME_ADJUSTMENT);
            float rotationalFollowSpeed = 1 / (_rotationalCameraLag / _LAG_DELTA_TIME_ADJUSTMENT);

            _rotationX = _inputReader._mouseDelta.y * _cameraInversion * _mouseSensitivity;
            _rotationY = _inputReader._mouseDelta.x * _mouseSensitivity;

            _newAngleX += _rotationX;
            _newAngleX = Mathf.Clamp(_newAngleX, _cameraTiltBounds.x, _cameraTiltBounds.y);
            _newAngleX = Mathf.Lerp(_lastAngleX, _newAngleX, rotationalFollowSpeed * Time.deltaTime);

            if (_isLockedOn)
            {
                Vector3 aimVector = _lockOnTarget.position - _playerTarget.position;
                Quaternion targetRotation = Quaternion.LookRotation(aimVector);
                targetRotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationalFollowSpeed * Time.deltaTime);
                _newAngleY = targetRotation.eulerAngles.y;
            }
            else
            {
                _newAngleY += _rotationY;
                _newAngleY = Mathf.Lerp(_lastAngleY, _newAngleY, rotationalFollowSpeed * Time.deltaTime);
            }

            _newPosition = _playerTarget.position;
            _newPosition = Vector3.Lerp(_lastPosition, _newPosition, positionalFollowSpeed * Time.deltaTime);

            transform.position = _newPosition;
            transform.eulerAngles = new Vector3(_newAngleX, _newAngleY, 0);

            _syntyCamera.localPosition = new Vector3(_cameraHorizontalOffset, _cameraHeightOffset, _cameraDistance * -1);
            _syntyCamera.localEulerAngles = new Vector3(_cameraTiltOffset, 0f, 0f);

            _lastPosition = _newPosition;
            _lastAngleX = _newAngleX;
            _lastAngleY = _newAngleY;
        }

        private void UpdateTopDownCamera()
        {
            Vector3 targetPosition = _playerTarget.position + _topDownOffset;
            _topDownCamera.transform.position = Vector3.Lerp(_topDownCamera.transform.position, targetPosition, _positionalCameraLagTopDown * Time.deltaTime);
            _topDownCamera.transform.rotation = _topDownRotation;
        }

        public void LockOn(bool enable, Transform newLockOnTarget)
        {
            _isLockedOn = enable;
            if (newLockOnTarget != null)
            {
                _lockOnTarget = newLockOnTarget;
            }
        }

        public void SetControlType(ControlType type)
        {
            _controlType = type;
            if (_controlType == ControlType.ThirdPerson)
            {
                _thirdPersonCamera.enabled = true;
                _topDownCamera.enabled = false;
            }
            else
            {
                _topDownCamera.enabled = true;
                _thirdPersonCamera.enabled = false;
            }
        }

        [ContextMenu("SetTopDownControlType")]
        public void SetTopDownControlType()
        {
            SetControlType(ControlType.TopDown);
        }

        [ContextMenu("SetThirdPersonControlType")]
        public void SetThirdPersonControlType()
        {
            SetControlType(ControlType.ThirdPerson);
        }

        public Vector3 GetCameraPosition()
        {
            return _controlType == ControlType.ThirdPerson ? _thirdPersonCamera.transform.position : _topDownCamera.transform.position;
        }

        public Vector3 GetCameraForward()
        {
            return _controlType == ControlType.ThirdPerson ? _thirdPersonCamera.transform.forward : _topDownCamera.transform.forward;
        }

        public Vector3 GetCameraForwardZeroedY()
        {
            Vector3 forward = GetCameraForward();
            return new Vector3(forward.x, 0, forward.z);
        }

        public Vector3 GetCameraForwardZeroedYNormalised()
        {
            return GetCameraForwardZeroedY().normalized;
        }

        public Vector3 GetCameraRightZeroedY()
        {
            Vector3 right = _controlType == ControlType.ThirdPerson ? _thirdPersonCamera.transform.right : _topDownCamera.transform.right;
            return new Vector3(right.x, 0, right.z);
        }

        public Vector3 GetCameraRightZeroedYNormalised()
        {
            return GetCameraRightZeroedY().normalized;
        }

        public float GetCameraTiltX()
        {
            return _controlType == ControlType.ThirdPerson ? _thirdPersonCamera.transform.eulerAngles.x : _topDownCamera.transform.eulerAngles.x;
        }
    }
}