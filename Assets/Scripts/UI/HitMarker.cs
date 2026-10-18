using System.Collections;
using UnityEngine;

/// <summary>
/// Class that handle the hit UI feedback. 
/// </summary>
public class HitMarker : MonoBehaviour
{
    public static HitMarker Instance { get; private set; }

    [Header("References")]
    [SerializeField] private RectTransform marker;
    [Space]
    [Header("Marker")]
    [SerializeField] private float showDuration = 0.15f;
    [SerializeField] private float popScale = 1.6f;
    [SerializeField] private float hitStopDuration = 0.03f;

    private float _hideTime;
    private AudioClip _tickClip;

    private void Awake()
    {
        Instance = this;
        // Set the marker inactive 
        marker.gameObject.SetActive(false);
        // Create the procedural
        _tickClip = CreateTickClip();
    }

    /// <summary>
    /// Trigger the hit UI.
    /// </summary>
    /// <param name="target"></param>
    public void ShowHit(Collider target)
    {
        // Activate the marker
        marker.gameObject.SetActive(true);
        // Pop the marker scale
        marker.localScale = Vector3.one * popScale;
        // Calculate the time to hide the marker
        _hideTime = Time.unscaledTime + showDuration;
        // Play tick audio
        AudioManager.Instance.PlaySound(_tickClip);
        // Freeze the time
        HitStop.Instance?.Freeze(hitStopDuration);
    }

    private void Update()
    {
        if (!marker.gameObject.activeSelf) return;

        marker.localScale = Vector3.Lerp(marker.localScale, Vector3.one, 20f * Time.unscaledDeltaTime);
        if (Time.unscaledTime >= _hideTime) marker.gameObject.SetActive(false);
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
