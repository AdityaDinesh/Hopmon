using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalView : MonoBehaviour
{
    [SerializeField] private Vector3 _rotationSpeed;

    private Transform _transform;

    private bool _isCollected;

    private void Awake()
    {
        _transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isCollected) return;

        _transform.Rotate(_rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !_isCollected)
        {
            _isCollected = true;
            PlayerController.Instance.StackCrystal(_transform);
        }
    }
}
