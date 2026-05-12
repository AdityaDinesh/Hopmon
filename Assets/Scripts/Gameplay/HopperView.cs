using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HopperView : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float wallCheckDistance = 1f;
    [SerializeField] private float _coolDownTime = 1f;

    private Vector3 currentDirection;

    private Transform _transform;
    private Animator _Animator;

    //private bool _canChangeDirection;
    private bool _isDead;
    private bool _isMoving;
    private bool _isInCooldown;

    private float _moveTimer;
    private float _coolDownTimer;
    private float _maxMoveTime;

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
        _Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        PickValidDirection();
        _isMoving = true;
        _moveTimer = 0f;
        _maxMoveTime = 1f / speed;
        //_Animator.speed = speed - 0.5f;
        _Animator.SetTrigger("jump");
    }

    // Update is called once per frame
    void Update()
    {
        if (_isDead) return;

        if (_isMoving)
        {

            _moveTimer += Time.deltaTime;
            _transform.position += currentDirection.normalized * speed * Time.deltaTime;

            if (_moveTimer > _maxMoveTime)
            {
                _moveTimer = 0f;
                _isMoving = false;
                _isInCooldown = true;
                _coolDownTimer = 0f;

                float newX = RoundUpToDecimal(_transform.position.x, 1);
                float newZ = RoundUpToDecimal(_transform.position.z, 1);
                _transform.position = new Vector3(newX, _transform.position.y, newZ);
                //_transform.position = new Vector3(newX, _transform.position.y, _transform.position.z);
            }
        }

        // Cooldown timer after moving
        if (_isInCooldown)
        {
            _coolDownTimer += Time.deltaTime;

            if (_coolDownTimer > _coolDownTime)
            {
                _coolDownTimer = 0f;
                _isInCooldown = false;
                _isMoving = true;
                _moveTimer = 0f;
                _maxMoveTime = 1f / speed;
                //_Animator.speed = speed - 0.5f;
                _Animator.SetTrigger("jump");
                PickValidDirection();
            }
        }

        Debug.DrawRay(_transform.position, currentDirection * wallCheckDistance, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fireball"))
        {
            _Animator.SetTrigger("dead");
            _isDead = true;
            Vector3 lookDirection = other.ClosestPoint(_transform.position) - _transform.position;
            _transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    private void PickValidDirection()
    {
        validCount = 0;

        // Collect all valid directions
        for (int i = 0; i < directions.Length; i++)
        {
            if (!IsBlockedAhead(directions[i]))
            {
                //if (directions[i] == -currentDirection || directions[i] == currentDirection) continue;
                if (directions[i] == -currentDirection) continue;

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

    private bool IsBlockedAhead(Vector3 dir)
    {
        Vector3 origin = _transform.position;

        // Forward ray
        if (Physics.Raycast(origin, dir, wallCheckDistance, obstacleLayer))
        {
            currentDirection *= -1;
            return true;
        }

        if (Physics.Raycast(origin, dir, wallCheckDistance, wallLayer))
            return true;

        return false;
    }

    public void PlayExplosionParticle()
    {
        PoolController.Instance.SpawnFromPool("Explosion", _transform.position, Quaternion.identity);
    }

    private float RoundUpToDecimal(float number, int numDecimalPlaces)
    {
        double multiplier = Math.Pow(10, numDecimalPlaces);
        // Multiply the number, round up using Mathf.Ceil, then divide back
        float result = Mathf.Ceil((float)(number * multiplier)) / (float)multiplier;
        return (Mathf.Round(result * 2f) / 2f);
    }
}
