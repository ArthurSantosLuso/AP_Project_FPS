using UnityEngine;

public class EyeFollow : MonoBehaviour
{

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        Follow();
    }

    private void Follow()
    {
        //Vector2 direction = new Vector2(
        //    Input.mousePosition.x - transform.position.x,
        //    Input.mousePosition.y - transform.position.y
        //    );

        //transform.up = direction;

        Vector3 mousePosition = _cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -_cam.transform.position.z));

        Vector2 direction = mousePosition - transform.position;

        transform.up = direction;
    }
}
