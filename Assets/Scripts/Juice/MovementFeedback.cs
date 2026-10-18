using UnityEngine;

[RequireComponent(typeof(FirstPersonMovement))]
public class MovementFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonMovement movement;
    [Space]
    [Header("Footsteps")]
    [SerializeField] private AudioClip[] footstepClips;
    [SerializeField] private float stepInterval = 0.42f;
    [Space]
    [Header("Landing")]
    [SerializeField] private AudioClip landingClip;
    [SerializeField] private float minLandingSpeed = 4f;
    [SerializeField] private float landingTraumaPerSpeed = 0.03f;
    [Space]
    [Header("Dash")]
    [SerializeField] private AudioClip dashClip;

    private float _stepTimer;
    private bool _wasGrounded;
    private bool _wasDashing;
    private float _fallSpeed;

    private void Start()
    {
        if (movement == null)
        {
            movement = GetComponent<FirstPersonMovement>();
        }       
    }

    private void Update()
    {
        UpdateFootsteps();
        UpdateLanding();
        UpdateDash();
    }

    private void UpdateFootsteps()
    {
        bool isWalking = movement.IsGrounded && !movement.IsDashing
            && movement.MovementInput.sqrMagnitude > 0.01f;

        if (!isWalking)
        {
            _stepTimer = 0f;
            return;
        }

        _stepTimer += Time.deltaTime;
        if (_stepTimer < stepInterval) return;

        _stepTimer = 0f;
        if (footstepClips.Length > 0)
            PlayAudio(footstepClips[Random.Range(0, footstepClips.Length)], 0.5f);
    }

    private void UpdateLanding()
    {
        if (!movement.IsGrounded) _fallSpeed = Mathf.Max(_fallSpeed, -movement.VerticalSpeed);

        if (movement.IsGrounded && !_wasGrounded && _fallSpeed >= minLandingSpeed)
        {
            CameraShake.Instance?.Shake(_fallSpeed * landingTraumaPerSpeed);
            PlayAudio(landingClip, Mathf.Clamp01(_fallSpeed / 15f));
        }

        if (movement.IsGrounded) _fallSpeed = 0f;
        _wasGrounded = movement.IsGrounded;
    }

    private void UpdateDash()
    {
        bool isDashing = movement.IsDashing;
        if (isDashing && !_wasDashing) PlayAudio(dashClip, 0.8f);

        ScreenFeedback.Instance?.SetDashing(isDashing);
        _wasDashing = isDashing;
    }

    private void PlayAudio(AudioClip clip, float volume)
    {
        if (clip == null) return;

        AudioManager.Instance.PlaySound(clip, Random.Range(0.9f, 1.1f));
    }
}
