using Utilities.Singletons;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class CameraController : Singleton<CameraController> {
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 20.0f;
	[Tooltip("How fast the character turns to face movement direction")]
	[Range(0.0f, 0.3f)]
	public float RotationSmoothTime = 0.12f;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;

	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 89.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = 5.0f;
	[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
	public float CameraAngleOverride = 0.0f;
	[Tooltip("For locking the camera position on all axis")]
	public bool LockCameraPosition = false;
	[Tooltip("For zooming in the camera")]
	public float MinZoom = 5.0f;
	[Tooltip("For zooming out the camera")]
	public float MaxZoom = 100.0f;

	// cinemachine
	public CinemachineVirtualCamera _virtualCamera;
	private float _cinemachineTargetYaw;
	private float _cinemachineTargetPitch = 38.0f;

	// player
	private float _speed;
	private float _targetRotation = 0.0f;
	private float _rotationVelocity;
	private float _verticalVelocity;

	private CharacterController _controller;
	private StarterAssetsInputs _input;
	private GameObject _mainCamera;

	private const float _threshold = 0.01f;

	private bool _isInitialized = false;

	private void Awake() {
		// get a reference to our main camera
		if (_mainCamera == null) {
			_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}

		_virtualCamera = _mainCamera.GetComponentInChildren<CinemachineVirtualCamera>();
	}

	private void Start() {
		_controller = GetComponent<CharacterController>();
		_input = GetComponent<StarterAssetsInputs>();
	}

	private void Update() {
		if (_isInitialized) Move();
	}

	private void LateUpdate() {
		if (_isInitialized) CameraRotation();
	}

	public void SetStartingPosition(Players player = Players.PLAYER_ONE) {
		if (player == Players.PLAYER_ONE) {
			CinemachineCameraTarget.transform.position = new Vector3(0, 2.0f, -10.0f);

			_cinemachineTargetYaw = 0.0f;
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
		}

		if (player == Players.PLAYER_TWO) {
			CinemachineCameraTarget.transform.position = new Vector3(0, 2.0f, 10.0f);

			_cinemachineTargetYaw = 180.0f;
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
		}

		if (player == Players.SPECTATOR) {
			CinemachineCameraTarget.transform.position = new Vector3(0, 2.0f, 10.0f);

			_cinemachineTargetYaw = 0.0f;
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
		}

		_isInitialized = true;
	}

	// Rotate while the right mouse is held down
	private void CameraRotation() {
		if (Mouse.current.rightButton.isPressed) {
			// if there is an input and camera position is not fixed
			if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition) {
				_cinemachineTargetYaw += _input.look.x *3;
				_cinemachineTargetPitch += _input.look.y *3;
			}

			// clamp our rotations so our values are limited 360 degrees
			_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
		}

		// Cinemachine will follow this target
		CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);

		Vector2 mousePosition = Camera.main.ScreenToViewportPoint(Mouse.current.position.ReadValue());
		if (mousePosition.x <= 0 || mousePosition.y <= 0 || mousePosition.x > 1 || mousePosition.y > 1) return;

		float zoom = Mouse.current.scroll.ReadValue().y;
		if (zoom != 0) {
			float fov = _virtualCamera.m_Lens.FieldOfView;
			float target = Mathf.Clamp(fov - zoom, MinZoom, MaxZoom);
			_virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(fov, target, 2.0f * Time.deltaTime);
		}
	}

	private void Move() {
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = MoveSpeed;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (_input.move == Vector2.zero) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

		float speedOffset = 0.1f;
		float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset) {
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		} else {
			_speed = targetSpeed;
		}

		// normalise input direction
		Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (_input.move != Vector2.zero) {
			_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
			float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

			// rotate to face input direction relative to camera position
			transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
		}


		Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

		// move the player
		// _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime));
		CinemachineCameraTarget.transform.position += targetDirection.normalized * (_speed * Time.deltaTime);
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax) {
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}
}