using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem.iOS;

public class Freezable : MonoBehaviour, IFreezable
{
    [SerializeField] float unfreezeGraceTime = 0.1f;
    [SerializeField] private Animator entityAnimator;
    // Add later the camp to stop navmesh in case of an enemy

    /// <summary>
    /// Get if entity is frozen or not
    /// </summary>
    public bool IsFrozen { get; private set; }

    private IDamageNotifier _notifier;
    private Coroutine _routine;
    private float _frozenAt;

    private void Awake() => _notifier = GetComponentInParent<IDamageNotifier>();
    
    private void OnEnable()
    {
        if (_notifier != null) _notifier.Damaged += OnDamaged;
    }

    private void OnDisable()
    {
        if (_notifier != null) _notifier.Damaged -= OnDamaged;
        if (IsFrozen) Unfreeze();
    }

    public void Freeze(float duration)
    {
        // If already frozen, stop it to freeze again
        if (_routine != null) StopCoroutine(_routine);
        // Store the freezing time
        _frozenAt = Time.time;
        // Start the freezing coroutine
        _routine = StartCoroutine(FreezeRoutine(duration));
    }

    public void Unfreeze()
    {
        // If not frozen, do nothing
        if (!IsFrozen) return;
        // If on frozen routine, stop it
        if (_routine != null) StopCoroutine(_routine);
        // Free the routine
        _routine = null;
        // Call method to handle unfreezing logic
        SetFrozen(false);
    }

    public void OnDamaged(float amount)
    {
        // Ignore damage from the same bullet that applied the freeze
        if (Time.time - _frozenAt < unfreezeGraceTime) return;
        Unfreeze();
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        SetFrozen(true);
        yield return new WaitForSeconds(duration);
        _routine = null;
        SetFrozen(false);
    }

    void SetFrozen(bool frozen)
    {
        IsFrozen = frozen;
        if (frozen)
        {
            if (entityAnimator != null ) entityAnimator.speed = 0f;
            foreach (PausableMonoBehaviour pausable in GetComponents<PausableMonoBehaviour>())
                pausable.Pause();
        }
        else
        {
            if (entityAnimator != null) entityAnimator.speed = 1f;
            foreach (PausableMonoBehaviour pausable in GetComponents<PausableMonoBehaviour>())
                pausable.UnPause();
        }
    }
}
