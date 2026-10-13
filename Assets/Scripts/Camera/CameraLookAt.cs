using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] private Camera _playerCamera;

    void Update()
    {
        if (_playerCamera != null)
        {
            transform.LookAt(transform.position + _playerCamera.transform.rotation * Vector3.forward, _playerCamera.transform.rotation * Vector3.up);
        }
        return;
    }
}
