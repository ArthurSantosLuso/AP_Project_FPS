using System.Collections;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    public static HitMarker Instance { get; private set; }

    [Header("References")]
    [SerializeField] private RectTransform _marker;
    [SerializeField] private AudioSource _audioSource;
    [Space]
    [Header("Marker")]
    [SerializeField] private float _showDuration = 0.15f;
    [SerializeField] private float _popScale = 1.6f;
    [SerializeField] private float _hitStopDuration = 0.03f;
    [SerializeField] private Color _flashColor = Color.white;
    [SerializeField] private float _targetFlashDuration = 0.06f;

    private float _hideTime;
    private AudioClip _tickClip;

    private void Awake()
    {
        Instance = this;
        _marker.gameObject.SetActive(false);
        _tickClip = CreateTickClip();
    }

    public void ShowHit(Collider target)
    {
        _marker.gameObject.SetActive(true);
        _marker.localScale = Vector3.one * _popScale;
        _hideTime = Time.unscaledTime + _showDuration;
        _audioSource.PlayOneShot(_tickClip);
        HitStop.Instance?.Freeze(_hitStopDuration);

        var targetRenderer = target.GetComponentInChildren<Renderer>();
        if (targetRenderer != null) StartCoroutine(FlashTarget(targetRenderer));
    }

    private void Update()
    {
        if (!_marker.gameObject.activeSelf) return;

        _marker.localScale = Vector3.Lerp(_marker.localScale, Vector3.one, 20f * Time.unscaledDeltaTime);
        if (Time.unscaledTime >= _hideTime) _marker.gameObject.SetActive(false);
    }

    private IEnumerator FlashTarget(Renderer targetRenderer)
    {
        var propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetColor("_BaseColor", _flashColor);
        propertyBlock.SetColor("_EmissionColor", _flashColor * 2f);
        targetRenderer.SetPropertyBlock(propertyBlock);

        yield return new WaitForSecondsRealtime(_targetFlashDuration);

        if (targetRenderer != null) targetRenderer.SetPropertyBlock(null);
    }

    // Short procedural "tick" so the marker works without an audio asset.
    private static AudioClip CreateTickClip()
    {
        const int SAMPLE_RATE = 44100;
        const int SAMPLE_COUNT = 1800;
        var samples = new float[SAMPLE_COUNT];
        for (int i = 0; i < SAMPLE_COUNT; i++)
        {
            float time = i / (float)SAMPLE_RATE;
            samples[i] = Mathf.Sin(2f * Mathf.PI * 2200f * time) * Mathf.Exp(-time * 90f) * 0.5f;
        }

        var clip = AudioClip.Create("HitTick", SAMPLE_COUNT, 1, SAMPLE_RATE, false);
        clip.SetData(samples, 0);
        return clip;
    }
}
