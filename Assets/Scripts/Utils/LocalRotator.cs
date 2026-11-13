using UnityEngine;

public class LocalRotator : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    [Header("Rotation")]
    [Tooltip("Which local axis to spin around.")]
    public Axis axis = Axis.Y;

    [Tooltip("Degrees per second. Negative values spin the other way.")]
    public float speed = 90f;

    private Quaternion startLocalRotation;
    private float angle;

    private void Awake()
    {
        // Remember the original local rotation so the other axes never change.
        startLocalRotation = transform.localRotation;
    }

    private void Update()
    {
        angle += speed * Time.deltaTime;

        Vector3 axisVector = axis switch
        {
            Axis.X => Vector3.right,
            Axis.Y => Vector3.up,
            _ => Vector3.forward
        };

        // Rotate around the chosen axis in the object's own local space,
        // on top of its starting local rotation.
        transform.localRotation = startLocalRotation * Quaternion.AngleAxis(angle, axisVector);
    }
}