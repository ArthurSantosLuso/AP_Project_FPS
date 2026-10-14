using UnityEngine;

public class MovementFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonMovement _movement;
    [SerializeField] private AudioSource _audioSource;
    [Space]
    [Header("Footsteps")]
    [SerializeField] private AudioClip[] _footstepClips;
    [SerializeField] private float _stepInterval = 0.42f;
    [Space]
    [Header("Landing")]
    [SerializeField] private AudioClip _landingClip;
    [SerializeField] private float _minLandingSpeed = 4f;
    [SerializeField] private float _landingTraumaPerSpeed = 0.03f;
    [Space]
    [Header("Dash")]
    [SerializeField] private AudioClip _dashClip;

    private float _stepTimer;
    private bool _wasGrounded;
    private bool _wasDashing;
    private float _fallSpeed;

    private void Update()
    {
        UpdateFootsteps();
        UpdateLanding();
        UpdateDash();
    }

    private void UpdateFootsteps()
    {
        bool isWalking = _movement.IsGrounded && !_movement.IsDashing
            && _movement.MovementInput.sqrMagnitude > 0.01f;

        if (!isWalking)
        {
            _stepTimer = 0f;
            return;
        }

        _stepTimer += Time.deltaTime;
        if (_stepTimer < _stepInterval) return;

        _stepTimer = 0f;
        if (_footstepClips.Length > 0)
            PlayClip(_footstepClips[Random.Range(0, _footstepClips.Length)], 0.5f);
    }

    private void UpdateLanding()
    {
        if (!_movement.IsGrounded) _fallSpeed = Mathf.Max(_fallSpeed, -_movement.VerticalSpeed);

        if (_movement.IsGrounded && !_wasGrounded && _fallSpeed >= _minLandingSpeed)
        {
            CameraShake.Instance?.Shake(_fallSpeed * _landingTraumaPerSpeed);
            PlayClip(_landingClip, Mathf.Clamp01(_fallSpeed / 15f));
        }

        if (_movement.IsGrounded) _fallSpeed = 0f;
        _wasGrounded = _movement.IsGrounded;
    }

    private void UpdateDash()
    {
        bool isDashing = _movement.IsDashing;
        if (isDashing && !_wasDashing) PlayClip(_dashClip, 0.8f);

        ScreenFeedback.Instance?.SetDashing(isDashing);
        _wasDashing = isDashing;
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        if (clip == null) return;

        _audioSource.pitch = Random.Range(0.9f, 1.1f);
        _audioSource.PlayOneShot(clip, volume);
    }
}
