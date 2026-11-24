using UnityEngine;

public class CollectBulletOnInteract : Interactable
{
    [Header("Collect Settings")]
    [SerializeField] private BulletData bulletToCollect;
    [SerializeField] private int amountToAdd;
    [SerializeField] private AudioClip collectAudio;
    [SerializeField] private bool breakWhenCollect = true;

    public override void TryInteract(Character entity = null)
    {
        if (entity == null) return;
        Interact(entity);
    }

    protected override void Interact(Character entity = null)
    {
        IAmmoReceiver receiver = entity.GetComponentInChildren<IAmmoReceiver>();
        if (receiver == null)
        {
            Debug.Log("receiver is null");
            return;
        }

        receiver.AddAmmo(bulletToCollect, amountToAdd);

        if (collectAudio != null)
            AudioManager.Instance.PlaySound(collectAudio);

        if (breakWhenCollect)
        {
            HidePrompt();
            Destroy(gameObject);
        }
    }
}
