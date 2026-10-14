using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    [SerializeField] private float _frozenTimeScale = 0.02f;

    private float _freezeEndTime;
    private bool _isFrozen;

    private void Awake()
    {
        Instance = this;
    }

    public void Freeze(float duration)
    {
        _freezeEndTime = Mathf.Max(_freezeEndTime, Time.unscaledTime + duration);
        if (!_isFrozen) StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        _isFrozen = true;
        float previousTimeScale = Time.timeScale;
        Time.timeScale = _frozenTimeScale;

        while (Time.unscaledTime < _freezeEndTime)
            yield return null;

        Time.timeScale = previousTimeScale;
        _isFrozen = false;
    }
}
