using UnityEngine;

/// <summary>
/// Universal script to handle the shake of the main camera
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private float maxAngle = 4f;
    [SerializeField] private float recoverSpeed = 18f;

    private float _trauma;
    private Vector3 _kick;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Shake the camera. Randon and jittery.
    /// </summary>
    /// <param name="trauma">Amount of shakiness</param>
    public void Shake(float trauma)
    {
        _trauma = Mathf.Clamp01(_trauma + trauma);
    }

    /// <summary>
    /// Quick and directional kick on the camera.
    /// </summary>
    /// <param name="eulerAngles"></param>
    public void Kick(Vector3 eulerAngles)
    {
        _kick += eulerAngles;
    }

    private void LateUpdate()
    {
        float shakeAmount = _trauma * _trauma * maxAngle;
        float time = Time.unscaledTime * 30f;
        var noise = new Vector3(
            Mathf.PerlinNoise(time, 0f) - 0.5f,
            Mathf.PerlinNoise(0f, time) - 0.5f,
            Mathf.PerlinNoise(time, time) - 0.5f) * 2f * shakeAmount;

        transform.localRotation = Quaternion.Euler(_kick + noise);

        float recover = 1f - Mathf.Exp(-recoverSpeed * Time.unscaledDeltaTime);
        _kick = Vector3.Lerp(_kick, Vector3.zero, recover);
        _trauma = Mathf.MoveTowards(_trauma, 0f, 2f * Time.unscaledDeltaTime);
    }
}
