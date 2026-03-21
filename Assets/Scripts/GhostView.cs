using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GhostView : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private Vector2 changeDirectionInterval;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float wallCheckDistance = 1f;
    [SerializeField]private Vector3 currentDirection;
    private float timer;
    private float currentChangeDirectionInterval;
    //private float _changeDirectionOffset = 0f;

    private Transform _transform;

    //private bool _canChangeDirection;
    private bool _isDead;

    private static readonly Vector3[] directions = new Vector3[]
    {
        Vector3.forward,
        Vector3.back,
        Vector3.left,
        Vector3.right
    };

    // Reusable buffer (no runtime allocation)
    private Vector3[] validDirections = new Vector3[4];
    private int validCount = 0;

    private void Awake()
    {
        _transform = transform;
    }

    private void Start()
    {
        PickValidDirection();
        currentChangeDirectionInterval = Random.Range(changeDirectionInterval.x, changeDirectionInterval.y);
    }

    private void Update()
    {
        if (_isDead) return;

        timer += Time.deltaTime;

        if (timer >= currentChangeDirectionInterval || IsWallAhead(currentDirection))
        {
            #region Old Code

            //if(IsWallAhead(currentDirection))
            //{
            //    _changeDirectionOffset = 0f;
            //}
            //else
            //{
            //    _changeDirectionOffset = 1f;
            //}
            //Debug.Log("Offset : " + _changeDirectionOffset);
            ////Debug.Log("Rounded x : " + RoundUpToDecimal(_transform.position.x, 1) + ", Rounded z :" + RoundUpToDecimal(_transform.position.z, 1));
            //float newX = RoundUpToDecimal(_transform.position.x, 1);
            //float newZ = RoundUpToDecimal(_transform.position.z, 1);

            //if(newX != _transform.position.x && Mathf.Abs(_transform.position.x) >= Mathf.Abs(newX) + (Mathf.Sign(newX) * _changeDirectionOffset))
            //{
            //    _canChangeDirection = true;
            //}
            //else
            //{
            //    if (newZ != _transform.position.z && Mathf.Abs(_transform.position.z) >= Mathf.Abs(newZ) + (Mathf.Sign(newZ) * _changeDirectionOffset))
            //    {
            //        _canChangeDirection = true;
            //    }
            //}

            //if(_canChangeDirection)
            //{
            //    PickValidDirection();
            //    timer = 0f;
            //    currentChangeDirectionInterval = Random.Range(changeDirectionInterval.x, changeDirectionInterval.y);
            //    _canChangeDirection = false;
            //    _transform.position = new Vector3(newX, _transform.position.y, newZ);
            //}

            //if(Mathf.Abs(_transform.position.x) >= Mathf.Abs(RoundUpToDecimal(_transform.position.x, 1)) && Mathf.Abs(_transform.position.z) >= Mathf.Abs(RoundUpToDecimal(_transform.position.z, 1)))
            //if (Mathf.Abs(_transform.position.x) >= Mathf.Abs(RoundUpToDecimal(_transform.position.x, 1)) && Mathf.Abs(_transform.position.z) >= Mathf.Abs(RoundUpToDecimal(_transform.position.z, 1)))
            //{
            //    PickValidDirection();
            //    timer = 0f;
            //    currentChangeDirectionInterval = Random.Range(changeDirectionInterval.x, changeDirectionInterval.y);
            //}

            #endregion

            if (Mathf.Approximately(GetFirstDigitAfterDecimal(_transform.position.x),5) && Mathf.Approximately(GetFirstDigitAfterDecimal(_transform.position.z), 5))
            {
                PickValidDirection();
                timer = 0f;
                currentChangeDirectionInterval = Random.Range(changeDirectionInterval.x, changeDirectionInterval.y);
                float newX = RoundUpToDecimal(_transform.position.x, 1);
                float newZ = RoundUpToDecimal(_transform.position.z, 1);
                _transform.position = new Vector3(newX, _transform.position.y, newZ);
            }

        }

        Move();
        RotateToDirection();
        Debug.DrawRay(_transform.position, currentDirection * wallCheckDistance, Color.red);
    }

    private void Move()
    {
        _transform.position += currentDirection * speed * Time.deltaTime;
    }

    private void PickValidDirection()
    {
        validCount = 0;

        // Collect all valid directions
        for (int i = 0; i < directions.Length; i++)
        {
            if (!IsWallAhead(directions[i]))
            {
                if (directions[i] == -currentDirection || directions[i] == currentDirection) continue;

                validDirections[validCount] = directions[i];
                validCount++;
            }
        }

        // If we found valid directions, pick one randomly
        if (validCount > 0)
        {
            currentDirection = validDirections[Random.Range(0, validCount)];
        }
        else
        {
            // Fallback (rare case: surrounded)
            currentDirection = -currentDirection;
        }
    }

    private bool IsWallAhead(Vector3 dir)
    {
        Ray ray = new Ray(_transform.position, dir);
        return Physics.Raycast(ray, wallCheckDistance, wallLayer);
    }

    private void RotateToDirection()
    {
        if (currentDirection != Vector3.zero)
        {
            _transform.rotation = Quaternion.LookRotation(currentDirection);
        }
    }

    // Round up decimal upto mentioned decimal point
    private float RoundUpToDecimal(float number, int numDecimalPlaces)
    {
        double multiplier = Math.Pow(10, numDecimalPlaces);
        // Multiply the number, round up using Mathf.Ceil, then divide back
        float result = Mathf.Ceil((float)(number * multiplier)) / (float)multiplier;
        return (Mathf.Round(result * 2f) / 2f);
    }

    private int GetFirstDigitAfterDecimal(float num)
    {
        return (int)(Mathf.Abs(num) * 10) % 10;
    }

}
