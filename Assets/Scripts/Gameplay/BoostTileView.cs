using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostTileView : MonoBehaviour
{
    [SerializeField] private Transform _destinationPoint;

    private bool _setDestinationPointOnPlayer;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !_setDestinationPointOnPlayer)
        {
            Vector3 boostDirectionVector = _destinationPoint.position - _transform.position;
            boostDirectionVector.y = 0;

            Debug.Log("Boost Tile Set Direction : " + boostDirectionVector);

            PlayerController.Instance.SetBoostMoveDirection(boostDirectionVector);
            _setDestinationPointOnPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && _setDestinationPointOnPlayer)
        {
            //_setDestinationPointOnPlayer = false;
        }
    }
}
