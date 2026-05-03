using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalView : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationSpeed;
    [SerializeField] private GameObject _shadowGameObject;
    [SerializeField] private float _flySpeed;

    [Header("Scatter Parameters")]
    [SerializeField] private float _explosionForce = 6f;
    [SerializeField] private float _upwardForce = 2f;
    [SerializeField] private float _radius = 2f;

    private Transform _transform;
    private Rigidbody _rigidBody;

    private bool _isCollected;
    private bool _canFly;
    private bool _isDead;

    private void Awake()
    {
        _transform = transform;
        _canFly = false;
        _isCollected = false;
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;

        if(!_canFly)
        {
            // Check if this crystal can fly
            if (PlayerController.Instance.ActiveFlyingCrystalTransform == _transform)
            {
                _canFly = true;
                _transform.SetParent(null);
            }
        }

        if (_isCollected && !_canFly) return;

        _transform.Rotate(_rotationSpeed * Time.deltaTime);

        if(_canFly)
        {
            _transform.position += Vector3.up * _flySpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !_isCollected)
        {
            _isCollected = true;
            PlayerController.Instance.StackCrystal(_transform, this);
            _shadowGameObject.SetActive(false);
        }

        if(other.CompareTag("Bound") || other.CompareTag("Ground") || other.CompareTag("Obstacle"))
        {
            Destroy(this);
        }
    }

    public void Scatter()
    {
        if (_rigidBody == null) return;

        Vector3 dir = (_transform.position - PlayerController.Instance.PlayerTransform.position).normalized;

        // Add randomness to direction
        dir += Random.insideUnitSphere * 0.5f;
        dir.Normalize();

        // Final force (horizontal + upward mix)
        Vector3 force = dir * _explosionForce + Vector3.up * _upwardForce;

        _transform.SetParent(null);
        _rigidBody.isKinematic = false;
        _rigidBody.useGravity = true;
        _rigidBody.AddForce(force, ForceMode.Impulse);
        _rigidBody.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
    }
}
