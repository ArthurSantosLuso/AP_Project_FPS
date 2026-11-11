using System;
using UnityEngine;

public class Condition : MonoBehaviour
{
    [SerializeField] private bool shouldConcludeWhenDestroyed = false;

    protected bool isConcluded = false;

    public bool IsConcluded => isConcluded;
    public event Action<Condition> OnConcluded;

    protected void SetConclusion(bool conclusion)
    {
        if (isConcluded == conclusion) return;
        isConcluded = conclusion;
        OnConcluded?.Invoke(this);
    }

    private void OnDestroy()
    {
        if (shouldConcludeWhenDestroyed && isConcluded != true)
            SetConclusion(true);
    }
}
