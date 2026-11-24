using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private bool isAutomatic = false;
    [SerializeField] private GameObject interactionHint;

    public bool IsAutomatic => isAutomatic;

    public abstract void TryInteract(Character entity = null);
    protected abstract void Interact(Character entity = null);

    public virtual void ShowPrompt()
    {
        if (interactionHint != null) interactionHint.SetActive(true);
    }

    public virtual void HidePrompt()
    {
        if (interactionHint != null) interactionHint.SetActive(false);
    }
}
