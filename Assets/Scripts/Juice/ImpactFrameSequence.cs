using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Anime impact frames over the whole scene: alternates an ink (black & white) look and a tinted look.
public class ImpactFrameSequence : MonoBehaviour
{
    public static ImpactFrameSequence Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Volume _inkVolume;
    [SerializeField] private Volume _tintVolume;
    [Space]
    [Header("Sequence")]
    [SerializeField] private float _frameDuration = 0.04f;
    [SerializeField] private int _frameCount = 4;

    private Coroutine _sequence;

    private void Awake()
    {
        Instance = this;
    }

    public void Play()
    {
        if (_sequence != null) StopCoroutine(_sequence);
        _sequence = StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        for (int i = 0; i < _frameCount; i++)
        {
            bool isTintFrame = i % 2 == 1;
            _inkVolume.weight = isTintFrame ? 0f : 1f;
            _tintVolume.weight = isTintFrame ? 1f : 0f;
            yield return new WaitForSecondsRealtime(_frameDuration);
        }

        _inkVolume.weight = 0f;
        _tintVolume.weight = 0f;
        _sequence = null;
    }
}
