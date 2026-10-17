using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float lifeTime;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= lifeTime) Destroy(gameObject);
    }
}
