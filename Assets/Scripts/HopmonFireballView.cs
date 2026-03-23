using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopmonFireballView : PoolObject
{
    [SerializeField] private float _speed;

    private Transform _transform;
    private GameObject _gameObject;

    private bool _isDead;
    private bool _canReturnToPool;

    private float _returnToPoolTimer;

    private void Awake()
    {
        _transform = transform;
        _gameObject = gameObject;
    }

    private void Update()
    {
        if(_canReturnToPool)
        {
            _returnToPoolTimer += Time.deltaTime;

            if(_returnToPoolTimer > 0.05f)
            {
                _canReturnToPool = false;
                _isDead = true;
                ReturnToPool(_gameObject);
            }
        }

        if (_isDead) return;

        _transform.position += _transform.forward * _speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle") || other.CompareTag("Enemy"))
        {
            if (_canReturnToPool || _isDead) return;

            // Particles

            _canReturnToPool = true;
            _returnToPoolTimer = 0f;
        }
    }

    public override void Setup()
    {
        base.Setup();

        _isDead = false;
        _canReturnToPool = false;
    }

    public override void ReturnToPool(GameObject _gameObject)
    {
        base.ReturnToPool(_gameObject);
    }
}
