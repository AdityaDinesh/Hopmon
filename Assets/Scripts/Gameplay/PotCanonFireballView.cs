using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotCanonFireballView : PoolObject
{
    [SerializeField] private float _landRadius;
    [SerializeField] private float _speed;
    [SerializeField] private float _archHeight;

    [SerializeField] private float _particleSpawnRate = 0.5f;

    private Transform _transform;
    private GameObject _gameObject;
    private Rigidbody _rigidBody;

    private bool _isDead;
    private bool _canReturnToPool;

    private float _returnToPoolTimer;
    private float _particleSpawnTimer;

    private void Awake()
    {
        _transform = transform;
        _gameObject = gameObject;
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (PlayerController.Instance.IsDead || (GameplayController.Instance.CurrentGameState == GameplayController.GameState.LevelEnd)
            && !_isDead && !_canReturnToPool && _gameObject.activeInHierarchy)
        {
            _canReturnToPool = true;
            _returnToPoolTimer = -0.25f;

            //_canReturnToPool = false;
            //ReturnToPool(_gameObject);
        }

        if (_canReturnToPool)
        {
            _returnToPoolTimer += Time.deltaTime;

            if (_returnToPoolTimer > 0.05f)
            {
                _canReturnToPool = false;
                _isDead = true;
                ReturnToPool(_gameObject);
            }
        }

        if (_isDead) return;

        _particleSpawnTimer += Time.deltaTime;

        if(_particleSpawnTimer > _particleSpawnRate)
        {
            PoolController.Instance.SpawnFromPool("PotCanonFireballTrail", _transform.position, Quaternion.identity);
            _particleSpawnTimer = 0f;
        }
    }

    private void FixedUpdate()
    {
        if (_isDead) return;

        Vector3 velocity = _rigidBody.velocity;

        // Ignore tiny velocities
        if (velocity.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(velocity.normalized);

        _transform.rotation = Quaternion.Slerp(
            _transform.rotation,
            targetRotation,
            1000000000 * Time.fixedDeltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle") || other.CompareTag("Player") || other.CompareTag("Bound"))
        {
            if (_canReturnToPool || _isDead) return;

            // Particles
            _isDead = true;
            _canReturnToPool = true;
            _returnToPoolTimer = 0f;
        }
    }

    public override void Setup()
    {
        base.Setup();

        _isDead = false;
        _canReturnToPool = false;


        Vector2 randomCircle = Random.insideUnitCircle * _landRadius;

        // Convert to XZ world position
        Vector3 randomPoint = new Vector3(
            _transform.position.x + randomCircle.x,
            _transform.position.y,
            _transform.position.z + randomCircle.y
        );

        Vector3 direction = randomPoint - _transform.position; 

        Vector3 targetPosition = _transform.position + direction.normalized * _landRadius;

        Vector3 velocity = CalculateBallisticVelocity(_transform.position, targetPosition, _archHeight);

        _rigidBody.velocity = velocity;
    }

    private Vector3 CalculateBallisticVelocity(Vector3 start, Vector3 target, float height)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);

        // Vertical distance
        float displacementY = target.y - start.y;

        // Horizontal distance
        Vector3 displacementXZ =
            new Vector3(
                target.x - start.x,
                0f,
                target.z - start.z
            );

        // Time to apex
        float timeUp =
            Mathf.Sqrt(2 * height / gravity);

        // Time to fall
        float timeDown =
            Mathf.Sqrt(
                2 * (height - displacementY) / gravity
            );

        float totalTime = timeUp + timeDown;

        // Vertical velocity
        Vector3 velocityY =
            Vector3.up *
            Mathf.Sqrt(2 * gravity * height);

        // Horizontal velocity
        Vector3 velocityXZ =
            displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }

    public override void ReturnToPool(GameObject _gameObject)
    {
        base.ReturnToPool(_gameObject);
    }
}
