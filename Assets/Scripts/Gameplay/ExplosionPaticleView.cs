using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionPaticleView : PoolObject
{
    private ParticleSystem _explosionParticleSystem;

    private GameObject _gameObject;
    private bool _canReturnToPool;
    private float _returnToPoolTimer;

    private void Awake()
    {
        _gameObject = gameObject;
        _explosionParticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (_canReturnToPool)
        {
            _returnToPoolTimer += Time.deltaTime;

            if (_returnToPoolTimer > 3f)
            {
                _canReturnToPool = false;
                _returnToPoolTimer = 0f;
                ReturnToPool(_gameObject);
            }
        }
    }

    public override void Setup()
    {
        base.Setup();

        _canReturnToPool = true;
        _explosionParticleSystem.Play();
    }

    public override void ReturnToPool(GameObject _gameObject)
    {
        _explosionParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        base.ReturnToPool(_gameObject);
    }
}
