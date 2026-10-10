using UnityEngine;

public abstract class Parryable : MonoBehaviour
{
    public virtual void Parry()
    {
        Destroy(gameObject); 
    }
}
