using UnityEngine;

/// <summary>
/// Base class for every ability's visual/audio feedback.
/// </summary>
public abstract class VFX : MonoBehaviour
{
    public abstract void Play();

    public virtual void Stop() { }
}
