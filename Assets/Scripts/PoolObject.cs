using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public string poolTag;

    public virtual void Setup()
    {

    }

    public virtual void ReturnToPool(GameObject _gameObject)
    {
        PoolController.Instance.ReturnToPool(poolTag, _gameObject);
    }
}
