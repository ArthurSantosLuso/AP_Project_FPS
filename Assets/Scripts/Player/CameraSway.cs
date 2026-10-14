using UnityEngine;

/// <summary>
/// Camera movement when walking and idling
/// </summary>
public class CameraSway : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonMovement _movement;
    [Space]
    [Header("Walk Sway")]
    [SerializeField] private float swayFrequency = 6f;
    [SerializeField] private float swayWidth = 0.03f;
    [SerializeField] private float swayRoll = 1f;
    [Space]
    [Header("Idle Breathing")]
    [SerializeField] private float breathFrequency = 1.5f;
    [SerializeField] private float breathHeight = 0.006f;
    [SerializeField] private float breathPitch = 0.3f;
    [SerializeField] private float blendSpeed = 6f;

    private float _swayTimer;
    private float _walkBlend;

    private void LateUpdate()
    {
        // Verify if player is walking (Dashing and air not included)
        bool isWalking = _movement.IsGrounded && !_movement.IsDashing
            && _movement.MovementInput.sqrMagnitude > 0.01f;

        // Math to calculate the camera sway
        _walkBlend = Mathf.Lerp(_walkBlend, isWalking ? 1f : 0f, blendSpeed * Time.deltaTime);
        if (isWalking) _swayTimer += swayFrequency * Time.deltaTime;

        float sway = Mathf.Sin(_swayTimer) * _walkBlend;
        float breath = Mathf.Sin(Time.time * breathFrequency) * (1f - _walkBlend);

        // Apply the sway to camera transform
        transform.localPosition = new Vector3(sway * swayWidth, breath * breathHeight, 0f);
        transform.localRotation = Quaternion.Euler(breath * breathPitch, 0f, -sway * swayRoll);
    }
}
