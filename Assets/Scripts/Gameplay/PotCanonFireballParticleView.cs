using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotCanonFireballParticleView : PoolObject
{
    private ParticleSystem _potCanonFireballParticleSystem;

    private GameObject _gameObject;
    private bool _canReturnToPool;
    private float _returnToPoolTimer;

    private void Awake()
    {
        _gameObject = gameObject;
        _potCanonFireballParticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (_canReturnToPool)
        {
            _returnToPoolTimer += Time.deltaTime;

            if (_returnToPoolTimer > 1f)
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
        _potCanonFireballParticleSystem.Play();
    }

    public override void ReturnToPool(GameObject _gameObject)
    {
        _potCanonFireballParticleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        base.ReturnToPool(_gameObject);
    }
}
