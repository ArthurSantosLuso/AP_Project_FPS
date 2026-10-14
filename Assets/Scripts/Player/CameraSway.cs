using UnityEngine;

public class CameraSway : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FirstPersonMovement _movement;
    [Space]
    [Header("Walk Sway")]
    [SerializeField] private float _swayFrequency = 6f;
    [SerializeField] private float _swayWidth = 0.03f;
    [SerializeField] private float _swayRoll = 1f;
    [Space]
    [Header("Idle Breathing")]
    [SerializeField] private float _breathFrequency = 1.5f;
    [SerializeField] private float _breathHeight = 0.006f;
    [SerializeField] private float _breathPitch = 0.3f;
    [SerializeField] private float _blendSpeed = 6f;

    private float _swayTimer;
    private float _walkBlend;

    private void LateUpdate()
    {
        bool isWalking = _movement.IsGrounded && !_movement.IsDashing
            && _movement.MovementInput.sqrMagnitude > 0.01f;

        _walkBlend = Mathf.Lerp(_walkBlend, isWalking ? 1f : 0f, _blendSpeed * Time.deltaTime);
        if (isWalking) _swayTimer += _swayFrequency * Time.deltaTime;

        float sway = Mathf.Sin(_swayTimer) * _walkBlend;
        float breath = Mathf.Sin(Time.time * _breathFrequency) * (1f - _walkBlend);

        transform.localPosition = new Vector3(sway * _swayWidth, breath * _breathHeight, 0f);
        transform.localRotation = Quaternion.Euler(breath * _breathPitch, 0f, -sway * _swayRoll);
    }
}
