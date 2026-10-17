using UnityEngine;

public class CollectBulletOnInteract : MonoBehaviour
{
    [SerializeField] private BulletData bulletToCollect;
    [SerializeField] private RevolverShot revolver;

    [SerializeField] private AudioClip collectAudio;

    private bool _canCollect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FirstPersonMovement>() != null)
        {
            _canCollect = true;
            Debug.Log("Can collect water bullet");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<FirstPersonMovement>() != null)
            _canCollect = false;
        Debug.Log("Cannot collect water bullet anymore");
    }

    private void Update()
    {
        if (_canCollect)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                revolver.AddBullet(bulletToCollect);
                AudioManager.Instance.PlaySound(collectAudio);
            }
        }
    }
}
