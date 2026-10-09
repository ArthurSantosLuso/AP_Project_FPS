using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    private Vector3 _velocity;
    private Vector3 _playerMovementInput;
    private Vector2 _playerMouseInput;
    private float _xRotation;

    [Header("Components Needed")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform player;
    [Space]
    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float sensetivity;
    [SerializeField] private float gravity = 9.81f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; 
    }

    // Update is called once per frame
    void Update()
    {

        _playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        _playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();
        MoveCamera();
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(_playerMovementInput);


        if (controller.isGrounded)
        {
            _velocity.y = -1f;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _velocity.y = jumpForce;
            }
        }
        else
        {
            _velocity.y += gravity * -2f * Time.deltaTime;
        }

        controller.Move(MoveVector * speed * Time.deltaTime);
        controller.Move(_velocity * Time.deltaTime);

    }
    private void MoveCamera()
    {
        _xRotation -= _playerMouseInput.y * sensetivity;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.Rotate(0f, _playerMouseInput.x * sensetivity, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
}
