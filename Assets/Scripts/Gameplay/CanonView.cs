using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanonView : MonoBehaviour
{
    [SerializeField] private float _shootInterval;
    [SerializeField] private Transform _firePointTransform;

    private float _shootTimer;

    // Update is called once per frame
    void Update()
    {
        _shootTimer += Time.deltaTime;

        if(_shootTimer >= _shootInterval)
        {
            _shootTimer = 0f;
            PoolController.Instance.SpawnFromPool("CanonFireball", _firePointTransform.position, _firePointTransform.rotation);
        }
    }
}
