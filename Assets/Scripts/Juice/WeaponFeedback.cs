using UnityEngine;

// Called by animation events on the arms clips (Shoot and Punch).
public class WeaponFeedback : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light _muzzleLight;
    [SerializeField] private Camera _playerCamera;
    [Space]
    [Header("Shoot")]
    [SerializeField] private Vector3 _shootKick = new Vector3(-2.5f, 0.6f, 0f);
    [SerializeField] private float _shootTrauma = 0.15f;
    [SerializeField] private float _shootFovKick = 3f;
    [SerializeField] private float _muzzleLightDuration = 0.05f;
    [Space]
    [Header("Punch")]
    [SerializeField] private Vector3 _punchKick = new Vector3(0f, 2f, -3f);
    [SerializeField] private float _punchTrauma = 0.35f;
    [SerializeField] private float _punchHitStopDuration = 0.1f;

    private float _muzzleLightTimer;

    public void OnShootFeedback()
    {
        CameraShake.Instance?.Kick(_shootKick);
        CameraShake.Instance?.Shake(_shootTrauma);
        _playerCamera.fieldOfView += _shootFovKick;
        _muzzleLight.enabled = true;
        _muzzleLightTimer = _muzzleLightDuration;
    }

    public void OnPunchHitFeedback()
    {
        CameraShake.Instance?.Kick(_punchKick);
        CameraShake.Instance?.Shake(_punchTrauma);
        HitStop.Instance?.Freeze(_punchHitStopDuration);
    }

    private void Update()
    {
        if (!_muzzleLight.enabled) return;

        _muzzleLightTimer -= Time.unscaledDeltaTime;
        if (_muzzleLightTimer <= 0f) _muzzleLight.enabled = false;
    }
}
