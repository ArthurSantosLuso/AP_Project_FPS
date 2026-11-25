using System.Collections.Generic;
using UnityEngine;

public class Interactor : PausableMonoBehaviour
{
    [Header("Facing")]
    [SerializeField] private Transform viewTransform;
    [SerializeField, Range(1f, 90f)] private float maxFacingAngle = 35f;

    [Header("Line of Sight")]
    [Tooltip("Layers that can block the interaction.")]
    [SerializeField] private LayerMask occlusionMask = ~0;

    private Character _character;
    private Interactable _currentInteractable;

    // Counter handles interactables that have more than one trigger collider
    private readonly Dictionary<Interactable, int> inRange = new();
    private readonly List<Interactable> toRemove = new();
    private readonly RaycastHit[] hitBuffer = new RaycastHit[16];

    private void Awake()
    {
        _character = GetComponent<Character>();
        if (viewTransform == null && Camera.main != null)
            viewTransform = Camera.main.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        Interactable interactable = other.gameObject.GetComponentInParent<Interactable>();
        if (interactable == null || !_character.ShouldConsiderInteractable) return;

        if (interactable.IsAutomatic)
        {
            // No need to be facing to interact
            interactable.TryInteract(_character);
            return;
        }

        inRange.TryGetValue(interactable, out int count);
        inRange[interactable] = count + 1;
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable interactable = other.GetComponentInParent<Interactable>();
        if (interactable == null || !inRange.TryGetValue(interactable, out int count)) return;

        if (count <= 1) inRange.Remove(interactable);
        else inRange[interactable] = count - 1;
    }

    private void Update()
    {
        if (GameState.IsPaused) return;
        SetCurrent(FindFacedInteractable());
    }

    private void SetCurrent(Interactable next)
    {
        if (next == _currentInteractable) return;

        if (_currentInteractable != null) _currentInteractable.HidePrompt();
        _currentInteractable = next;
        if (_currentInteractable != null) _currentInteractable.ShowPrompt();
    }

    private Interactable FindFacedInteractable()
    {
        if (!_character.ShouldConsiderInteractable) return null;

        Interactable best = null;
        float bestAngle = maxFacingAngle;
        toRemove.Clear();

        foreach (var kv in inRange)
        {
            Interactable interactable = kv.Key;
            if (interactable == null)
            {
                toRemove.Add(interactable);
                continue;
            }

            Vector3 dir = interactable.InteractionPoint - viewTransform.position;
            float angle = Vector3.Angle(viewTransform.forward, dir);

            // Make a cheap angle test first, raycast only if it could become the best candidate
            if (angle > bestAngle) continue;
            if (!HasLineOfSight(interactable)) continue;

            bestAngle = angle;
            best = interactable;
        }

        foreach (var dead in toRemove) inRange.Remove(dead);
        return best;
    }

    private bool HasLineOfSight(Interactable target)
    {
        Vector3 origin = viewTransform.position;
        Vector3 toTarget = target.InteractionPoint - origin;
        float distance = toTarget.magnitude;
        if (distance < 0.01f) return true;

        int count = Physics.RaycastNonAlloc(
            origin, toTarget / distance, hitBuffer, distance,
            occlusionMask, QueryTriggerInteraction.Ignore);

        // Find closest hit that isn't the player itself
        float closest = float.MaxValue;
        Collider closestCol = null;
        for (int i = 0; i < count; i++)
        {
            Collider col = hitBuffer[i].collider;
            if (col.transform.IsChildOf(transform)) continue; // Ignore own colliders

            if (hitBuffer[i].distance < closest)
            {
                closest = hitBuffer[i].distance;
                closestCol = col;
            }
        }

        // Nothing in the way, or the first thing is the interactable itself
        if (closestCol == null) return true;
        return closestCol.GetComponentInParent<Interactable>() == target;
    }

    public void ExecuteInteraction()
    {
        if (_currentInteractable != null)
        {
            _currentInteractable.TryInteract(_character);
        }
    }
}