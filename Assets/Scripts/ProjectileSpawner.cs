using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject shotPoint;

    private float _timer;

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer > 1.5f)
        {
            Instantiate(prefab, shotPoint.transform.position, shotPoint.transform.rotation);
            _timer = 0;
        }
    }
}
