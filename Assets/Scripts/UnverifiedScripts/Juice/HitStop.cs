using System.Collections;
using UnityEngine;

/// <summary>
/// Handles the few fames freeze after impact of heavy action.
/// </summary>
public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    [SerializeField] private float frozenTimeScale = 0.02f;

    private float _freezeEndTime;
    private bool _isFrozen;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Freezes the game.
    /// </summary>
    /// <param name="duration">Time (seconds) the freeze will last.</param>
    public void Freeze(float duration)
    {
        // Calculate beforehand the end time for the freeze
        _freezeEndTime = Mathf.Max(_freezeEndTime, Time.unscaledTime + duration);
        // Just freeze the game if now currently frozen
        if (!_isFrozen) StartCoroutine(FreezeRoutine());
    }

    private IEnumerator FreezeRoutine()
    {
        // Check the game frozen flag
        _isFrozen = true; 
        // Store regular game time scale
        float previousTimeScale = Time.timeScale;
        // Set the frozen time scale
        Time.timeScale = frozenTimeScale;

        // Freeze lasts the given amount of time
        while (Time.unscaledTime < _freezeEndTime)
            yield return null;

        // Restore the regular time scale of the game
        Time.timeScale = previousTimeScale;
        // Un-check the game frozen flag
        _isFrozen = false; 
    }
}
