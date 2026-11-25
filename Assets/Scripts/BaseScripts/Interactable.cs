using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private bool isAutomatic = false;
    [SerializeField] private string interactionName;
    [Tooltip("Optional. Point the player must look at. Defaults to this transform.")]
    [SerializeField] private Transform interactionPoint;

    public bool IsAutomatic => isAutomatic;
    public Vector3 InteractionPoint => interactionPoint != null ? interactionPoint.position : transform.position;

    public abstract void TryInteract(Character entity = null);
    protected abstract void Interact(Character entity = null);

    public virtual void ShowPrompt() => UIHandler.Instance.ShowInteractionHint(interactionName);

    public virtual void HidePrompt() => UIHandler.Instance.HideInteractionHint();
}
