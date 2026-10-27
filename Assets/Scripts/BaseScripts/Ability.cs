using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    [Tooltip("Audio to be played when the ability is performed.")]
    [SerializeField]
    protected AudioClip actionAudio;
    [Tooltip("Audio to be played when the ability cannot be performed.")]
    [SerializeField]
    protected AudioClip actionFailedAudio;

    [Header("On Hit Effects Propreties")]
    [SerializeField]
    protected float shakeIntensity = 0.5f;
    [SerializeField]
    protected float shakeDuration = 0.2f;
    [SerializeField]
    protected float shakeDelay = 0.2f;
    [SerializeField]
    protected float onHitTimeScale = 0.02f;
    [SerializeField]
    protected float onHitTimeScaleDuration = 0.05f;

    protected Character owner;
    protected Animator animator;


    private void Awake()
    {
        owner = GetComponentInParent<Character>()
        ?? GetComponent<Character>()
        ?? GetComponentInChildren<Character>();

        animator = owner.GetComponent<Animator>();
    }

    public virtual void Initialize(Character owner, Animator animator)
    {
        this.owner = owner;
        this.animator = animator;
    }

    protected void PlayActionAudio()
    {
        if (actionAudio == null)
        {
            Debug.Log($"{name} has no 'actionAudio' to be played");
        }
        else
        {
            AudioManager.Instance.PlaySound(actionAudio, Random.Range(0.9f, 1.1f));
        }
    }

    protected void PlayActionFailedAudio()
    {
        if (actionAudio == null)
        {
            Debug.Log($"{name} has no 'actionFailedAudio' to be played");
        }
        else
        {
            AudioManager.Instance.PlaySound(actionFailedAudio, Random.Range(0.9f, 1.1f));
        }
    }

    public abstract void Perform();
    protected abstract bool CanAttack();

    public virtual void EnableHitbox() { }
    public virtual void DisableHitbox() { }

    public virtual void DeployProjectile() { }
}
