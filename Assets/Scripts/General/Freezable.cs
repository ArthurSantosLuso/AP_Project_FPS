using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Freezable : MonoBehaviour, IFreezable
{
    [SerializeField] float unfreezeGraceTime = 0.1f;
    [SerializeField] private Animator entityAnimator;
    [SerializeField] private GameObject frozenVFX;
    [SerializeField] private NavMeshAgent agent;

    private GameObject _currentFrozenVFXGameObject;

    /// <summary>
    /// Get if entity is frozen or not
    /// </summary>
    public bool IsFrozen { get; private set; }

    private IDamageNotifier _notifier;
    private Coroutine _routine;
    private float _frozenAt;

    private void Awake()
    {
        _notifier = GetComponentInParent<IDamageNotifier>();
        agent = GetComponentInParent<NavMeshAgent>();
    }
    
    private void OnEnable()
    {
        if (_notifier != null) _notifier.Damaged += OnDamaged;
    }

    private void OnDisable()
    {
        if (_notifier != null) _notifier.Damaged -= OnDamaged;

        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        if (IsFrozen)
        {
            // Object is being disabled/destroyed, skip agent logic, just reset state
            IsFrozen = false;
            if (entityAnimator != null) entityAnimator.speed = 1f;
            if (_currentFrozenVFXGameObject != null) Destroy(_currentFrozenVFXGameObject);
            _currentFrozenVFXGameObject = null;
        }
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

            AgentStopper.SetAgentState(agent, false);

            if (entityAnimator != null) entityAnimator.speed = 0f;
            foreach (PausableMonoBehaviour pausable in GetComponents<PausableMonoBehaviour>())
                pausable.Pause();

            _currentFrozenVFXGameObject = Instantiate(frozenVFX, transform.position, transform.rotation, transform);
        }
        else
        {
            AgentStopper.SetAgentState(agent, true);

            if (entityAnimator != null) entityAnimator.speed = 1f;
            foreach (PausableMonoBehaviour pausable in GetComponents<PausableMonoBehaviour>())
                pausable.UnPause();

            if (_currentFrozenVFXGameObject != null) Destroy(_currentFrozenVFXGameObject);
            _currentFrozenVFXGameObject = null;
        }
    }
}
