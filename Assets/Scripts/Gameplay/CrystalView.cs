using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalView : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationSpeed;
    [SerializeField] private GameObject _shadowGameObject;
    [SerializeField] private float _flySpeed;

    private Transform _transform;

    private bool _isCollected;
    private bool _canFly;

    private void Awake()
    {
        _transform = transform;
        _canFly = false;
        _isCollected = false;
    }

    // Update is called once per frame
    void Update()
    {
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
            PlayerController.Instance.StackCrystal(_transform);
            _shadowGameObject.SetActive(false);
        }

        if(other.CompareTag("Bound"))
        {
            Destroy(this);
        }
    }
}
