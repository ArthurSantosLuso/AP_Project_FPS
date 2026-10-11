using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonMovement movement;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private CanvasGroup speedLines;
    [Space]
    [Header("Field Of View")]
    [SerializeField] private float dashFovBoost = 15f;
    [SerializeField] private float aimFov = 45f;
    [SerializeField] private float fovSpeed = 12f;
    [Space]
    [Header("Tilt")]
    [SerializeField] private float strafeTilt = 2.5f;
    [SerializeField] private float dashTilt = 6f;
    [SerializeField] private float tiltSpeed = 10f;
    [Space]
    [Header("Head Bob")]
    [SerializeField] private float bobFrequency = 12f;
    [SerializeField] private float bobHeight = 0.04f;
    [SerializeField] private float bobSmoothing = 10f;
    [Space]
    [Header("Landing")]
    [SerializeField] private float landingDipPerSpeed = 0.02f;
    [SerializeField] private float maxLandingDip = 0.3f;
    [SerializeField] private float landingRecoverSpeed = 8f;
    [Space]
    [Header("Speed Lines")]
    [SerializeField] private float speedLinesFadeSpeed = 15f;

    private Vector3 _startLocalPosition;
    private float _normalFov;
    private float _tilt;
    private float _bobTimer;
    private float _bobOffset;
    private float _landingDip;
    private bool _wasGrounded;

    private void Awake()
    {
        _startLocalPosition = transform.localPosition;
        _normalFov = playerCamera.fieldOfView;
    }

    private void LateUpdate()
    {
        UpdateFieldOfView();
        UpdateTilt();
        UpdateHeadBob();
        UpdateLandingDip();
        UpdateSpeedLines();

        transform.localPosition = _startLocalPosition + Vector3.up * (_bobOffset - _landingDip);
        transform.localRotation = Quaternion.Euler(0f, 0f, _tilt);
    }

    private void UpdateFieldOfView()
    {
        float targetFov = _normalFov;
        if (movement.IsDashing) targetFov = _normalFov + dashFovBoost;
        else if (playerInput.AimHeld) targetFov = aimFov;

        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, fovSpeed * Time.deltaTime);
    }

    private void UpdateTilt()
    {
        float tiltAmount = movement.IsDashing ? dashTilt : strafeTilt;
        float targetTilt = -movement.MovementInput.x * tiltAmount;

        _tilt = Mathf.Lerp(_tilt, targetTilt, tiltSpeed * Time.deltaTime);
    }

    private void UpdateHeadBob()
    {
        bool isWalking = movement.IsGrounded && !movement.IsDashing
            && movement.MovementInput.sqrMagnitude > 0.01f;

        if (isWalking) _bobTimer += bobFrequency * Time.deltaTime;
        float targetOffset = isWalking ? Mathf.Sin(_bobTimer) * bobHeight : 0f;

        _bobOffset = Mathf.Lerp(_bobOffset, targetOffset, bobSmoothing * Time.deltaTime);
    }

    private void UpdateLandingDip()
    {
        if (movement.IsGrounded && !_wasGrounded)
            _landingDip = Mathf.Clamp(-movement.VerticalSpeed * landingDipPerSpeed, 0f, maxLandingDip);

        _wasGrounded = movement.IsGrounded;
        _landingDip = Mathf.Lerp(_landingDip, 0f, landingRecoverSpeed * Time.deltaTime);
    }

    private void UpdateSpeedLines()
    {
        float targetAlpha = movement.IsDashing ? 1f : 0f;
        speedLines.alpha = Mathf.Lerp(speedLines.alpha, targetAlpha, speedLinesFadeSpeed * Time.deltaTime);

        if (movement.IsDashing)
            speedLines.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    }
}