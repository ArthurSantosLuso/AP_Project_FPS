using UnityEngine;

// After a successful parry the left hand carries the energy until it touches the gun.
// OnParryPowerTransfer is called by an animation event on the Parry clip.
public class ParryPowerTransfer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _hand;
    [SerializeField] private Transform _gunCylinder;
    [SerializeField] private GameObject _chargePrefab;
    [SerializeField] private GameObject _dischargePrefab;
    [SerializeField] private Light _gunLight;
    [Space]
    [Header("Feel")]
    [SerializeField] private float _maxChargeTime = 2.5f;
    [SerializeField] private float _dischargeScale = 0.3f;
    [SerializeField] private float _dischargeHitStop = 0.05f;
    [SerializeField] private float _dischargeTrauma = 0.3f;
    [SerializeField] private Color _gunLightColor = new Color(0.4f, 0.8f, 1f);
    [SerializeField] private float _gunLightDuration = 0.35f;

    private GameObject _charge;
    private float _chargeEndTime;
    private float _gunLightEndTime;

    public void Charge()
    {
        if (_charge == null) _charge = Instantiate(_chargePrefab, _hand, false);
        _chargeEndTime = Time.time + _maxChargeTime;
    }

    public void OnParryPowerTransfer()
    {
        if (_charge == null) return;

        ReleaseCharge();

        GameObject discharge = Instantiate(_dischargePrefab, _gunCylinder.position, _gunCylinder.rotation);
        discharge.transform.localScale = Vector3.one * _dischargeScale;
        Destroy(discharge, 2f);

        _gunLight.color = _gunLightColor;
        _gunLight.enabled = true;
        _gunLightEndTime = Time.unscaledTime + _gunLightDuration;

        HitStop.Instance?.Freeze(_dischargeHitStop);
        CameraShake.Instance?.Shake(_dischargeTrauma);
    }

    private void Update()
    {
        if (_charge != null && Time.time >= _chargeEndTime) ReleaseCharge();
        if (_gunLight.enabled && Time.unscaledTime >= _gunLightEndTime) _gunLight.enabled = false;
    }

    // Detaches the charge so its trail fades out in place instead of popping.
    private void ReleaseCharge()
    {
        _charge.transform.SetParent(null, true);
        foreach (ParticleSystem particles in _charge.GetComponentsInChildren<ParticleSystem>())
            particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        foreach (TrailRenderer trail in _charge.GetComponentsInChildren<TrailRenderer>())
            trail.emitting = false;

        Destroy(_charge, 1f);
        _charge = null;
    }
}
