using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballCollideParticleView : PoolObject
{
    private ParticleSystem _fireballCollideParticleSystem;

    private GameObject _gameObject;
    private bool _canReturnToPool;
    private float _returnToPoolTimer;

    private void Awake()
    {
        _gameObject = gameObject;
        _fireballCollideParticleSystem = GetComponent<ParticleSystem>();
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
        _fireballCollideParticleSystem.Play();
    }

    public override void ReturnToPool(GameObject _gameObject)
    {
        _fireballCollideParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        base.ReturnToPool(_gameObject);
    }
}
