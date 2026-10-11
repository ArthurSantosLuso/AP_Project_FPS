using UnityEngine;

public class EnemyProjectile : Parryable
{
    [SerializeField] private float speed;

    private float _timeAlive;

    private void Update()
    {
        _timeAlive += Time.deltaTime;
        if (_timeAlive >= 1.5f)
        {
            Destroy(gameObject);
            return;
        }

        float stepDistance = speed * Time.deltaTime;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        transform.position = origin + direction * stepDistance;
    }

    public override void Parry()
    {
        base.Parry();
    }
}
