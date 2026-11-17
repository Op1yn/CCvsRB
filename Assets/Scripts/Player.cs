using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float gravity = -20f;

    private const float MaxInputMagnitude = 1f;
    private const float GroundStickForce = -2f;

    private CharacterController _controller;
    private InputSystem_Actions _controls;
    private Vector2 _moveInput;
    private Vector3 _velocity;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _controls = new InputSystem_Actions();

        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => _moveInput = Vector2.zero;
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    private void Update()
    {
        Vector3 movement = Vector3.ClampMagnitude(new Vector3(_moveInput.x, 0f, _moveInput.y), MaxInputMagnitude) * moveSpeed;

        if (_controller.isGrounded && _velocity.y < 0f)
            _velocity.y = GroundStickForce;

        _velocity.y += gravity * Time.deltaTime;
        _controller.Move((movement + Vector3.up * _velocity.y) * Time.deltaTime);

        if (movement.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.LookRotation(movement.normalized), rotationSpeed * Time.deltaTime);
    }
}
