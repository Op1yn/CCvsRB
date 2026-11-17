using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bot : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float _moveSpeed = 4f;
    [SerializeField] private float _stopDistance = 2f;
    [SerializeField] private float _rotationSpeed = 540f;
    [SerializeField] private float _stepHeight = 0.4f;
    [SerializeField] private float _stepSmooth = 0.2f;
    [SerializeField] private float _stepEpsilon = 0.05f;
    [SerializeField] private CapsuleCollider _capsuleCollider;

    [Header("Проверка земли")]
    [SerializeField] private float _rayDownDistance = 0.6f;
    [SerializeField] private LayerMask _groundMask;

    [Header("Преследование")]
    [SerializeField] private Transform Player;


    private Rigidbody _rigidbody;
    private float _colliderRadius;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _colliderRadius = _capsuleCollider.radius;
    }

    private void FixedUpdate()
    {
        Vector3 toPlayer = Player.position - transform.position;
        toPlayer.y = 0f;
        float distdistance = toPlayer.magnitude;

        Vector3 velocity = Vector3.zero;

        if (distdistance > _stopDistance)
        {
            Vector3 direction = toPlayer.normalized;
            velocity = direction * _moveSpeed;
            StepUp();
        }

        _rigidbody.linearVelocity = new Vector3(velocity.x, _rigidbody.linearVelocity.y, velocity.z);

        if (velocity.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(velocity.normalized);
            _rigidbody.rotation = Quaternion.RotateTowards(_rigidbody.rotation, target, _rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void StepUp()
    {
        Vector3 origin = transform.position + Vector3.up * _stepHeight + transform.forward * _colliderRadius;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, _rayDownDistance, _groundMask) && hit.point.y > transform.position.y + _stepEpsilon)
        {
            _rigidbody.position += Vector3.up * _stepSmooth * Time.fixedDeltaTime * (_stepHeight / Mathf.Max(_stepSmooth, 0.01f));
        }
    }
}
