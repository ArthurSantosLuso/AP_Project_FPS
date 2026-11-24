using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private bool isAutomatic = false;
    [SerializeField] private string interactionName;
    // [SerializeField] private GameObject interactionHint;

    public bool IsAutomatic => isAutomatic;

    public abstract void TryInteract(Character entity = null);
    protected abstract void Interact(Character entity = null);

    public virtual void ShowPrompt() => UIHandler.Instance.ShowInteractionHint(interactionName);

    public virtual void HidePrompt() => UIHandler.Instance.HideInteractionHint();
}
