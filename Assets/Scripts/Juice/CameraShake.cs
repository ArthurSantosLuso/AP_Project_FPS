using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private float _maxAngle = 4f;
    [SerializeField] private float _recoverSpeed = 18f;

    private float _trauma;
    private Vector3 _kick;

    private void Awake()
    {
        Instance = this;
    }

    public void Shake(float trauma)
    {
        _trauma = Mathf.Clamp01(_trauma + trauma);
    }

    public void Kick(Vector3 eulerAngles)
    {
        _kick += eulerAngles;
    }

    private void LateUpdate()
    {
        float shakeAmount = _trauma * _trauma * _maxAngle;
        float time = Time.unscaledTime * 30f;
        var noise = new Vector3(
            Mathf.PerlinNoise(time, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, time) - 0.5f,
            Mathf.PerlinNoise(time, time) - 0.5f) * 2f * shakeAmount;

        transform.localRotation = Quaternion.Euler(_kick + noise);

        float recover = 1f - Mathf.Exp(-_recoverSpeed * Time.unscaledDeltaTime);
        _kick = Vector3.Lerp(_kick, Vector3.zero, recover);
        _trauma = Mathf.MoveTowards(_trauma, 0f, 2f * Time.unscaledDeltaTime);
    }
}
