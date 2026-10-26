using UnityEngine;

public class Floater : MonoBehaviour
{
    Vector3 rotation;
    Vector3 currentRotation;
    float timer;

    [Header("Rotação")]
    [SerializeField] Vector2 rotationStrenghtX = new Vector2(-1f, 1f);
    [SerializeField] Vector2 rotationStrenghtY = new Vector2(-1f, 1f);
    [SerializeField] Vector2 rotationStrenghtZ = new Vector2(-1f, 1f);

    [Header("Controlo da orientaçao")]
    [SerializeField] float directionChangeInterval = 5f;
    [SerializeField] float rotationVariationSmoothness = 10f;

    private void Start()
    {
        ChooseNewRotation();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= directionChangeInterval)
        {
            ChooseNewRotation();
            timer = 0f;
        }

        currentRotation = Vector3.MoveTowards(currentRotation, rotation, rotationVariationSmoothness * Time.deltaTime);
        transform.Rotate(currentRotation * Time.deltaTime, Space.Self);
    }

    private void ChooseNewRotation()
    {
        rotation = new Vector3(Random.Range(rotationStrenghtX.x, rotationStrenghtX.y), Random.Range(rotationStrenghtY.x, rotationStrenghtY.y), Random.Range(rotationStrenghtZ.x, rotationStrenghtZ.y));
    }
}