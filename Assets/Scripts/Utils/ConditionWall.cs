using UnityEngine;
using System.Collections.Generic;

public class ConditionWall : MonoBehaviour
{
    [SerializeField] private bool shouldBeDestroyedToCount = true;
    [SerializeField] private List<Condition> conditions;

    private int concludedCount;

    private void OnEnable()
    {
        concludedCount = 0;
        foreach (Condition condition in conditions)
        {
            if (condition.IsConcluded) concludedCount++;
            condition.OnConcluded += HandleConditionChanged;
        }
        CheckAll();
    }

    private void OnDisable()
    {
        foreach (Condition condition in conditions)
            if (condition != null) condition.OnConcluded -= HandleConditionChanged;
    }

    private void HandleConditionChanged(Condition condition)
    {
        concludedCount += condition.IsConcluded ? 1 : -1;
        CheckAll();
    }

    private void CheckAll()
    {
        if (concludedCount >= conditions.Count)
            gameObject.SetActive(false);
    }
}
