using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float lifeTime;

    private float _timer;

    private void Update()
    {
        if (GameState.IsPaused) return;

        _timer += Time.deltaTime;

        if (_timer >= lifeTime) Destroy(gameObject);
    }
}
