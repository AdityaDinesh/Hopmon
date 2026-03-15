using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Terresquall;
using System;

public class PlayerController : MonoBehaviour
{

    public static PlayerController Instance;

    [SerializeField]private float _speed = 5f;
    [SerializeField]private Transform _cameraTransform;
    [SerializeField]private LayerMask _obstacleLayer;
    [SerializeField]private float _wallCheckDistance = 10f;

    [SerializeField] private Transform _fireballAimTransform;
    [SerializeField] private Transform _crystalStackTransform;
    [SerializeField] private float _crystalStackYOffset = 1f;

    private Vector3 _lastMoveDirection = Vector3.zero;

    public Transform PlayerTransform
    {
        get { return _transform; }
    }
    private Transform _transform;
    private Animator _animator;
    private bool _isMoving;
    private bool _isInCooldown;
    private bool _canShoot;

    private float _moveTimer;
    private float _coolDownTimer;
    private float _shootTimer;
    private float _maxMoveTime;
    private float horizontal;
    private float vertical;

    private Vector3 _crystalStackOffset;
    private Vector3 move;

    private void Awake()
    {
        // Check if an instance already exists
        if (Instance != null && Instance != this)
        {
            // If another instance exists, destroy this one
            Destroy(this.gameObject);
        }
        else
        {
            // If no instance exists, set this one as the instance
            Instance = this;
            // Optional: keep the object alive across scene loads
            DontDestroyOnLoad(this.gameObject);
        }

        _transform = transform;
        _animator = GetComponent<Animator>();
        _maxMoveTime = 1f / _speed;
        _animator.speed = _speed - 0.5f;
        _crystalStackOffset = Vector3.zero;
    }

    void Update()
    {
        // PLayer is Moving
        if (_isMoving)
        {
            _moveTimer += Time.deltaTime;
            _transform.position += move.normalized * _speed * Time.deltaTime;

            if (_moveTimer > _maxMoveTime)
            {
                _moveTimer = 0f;
                _isMoving = false;
                horizontal = 0f;
                vertical = 0f;
                _isInCooldown = true;
                _coolDownTimer = 0f;

                float newX = RoundUpToDecimal(_transform.position.x, 1);
                float newZ = RoundUpToDecimal(_transform.position.z, 1);
                _transform.position = new Vector3(newX, _transform.position.y, newZ);
                //_transform.position = new Vector3(newX, _transform.position.y, _transform.position.z);
            }
        }

        // Cooldown timer after moving
        if(_isInCooldown)
        {
            _coolDownTimer += Time.deltaTime;

            if(_coolDownTimer > 0.03f)
            {
                _coolDownTimer = 0f;
                _isInCooldown = false;
            }
        }

        // Take inputs if player is not moving, in cooldown or camera is not rotating
        if(!_isMoving && !_isInCooldown && !CameraController.Instance.IsMoving)
        {
            horizontal = VirtualJoystick.GetAxis("Horizontal", 0);
            vertical = VirtualJoystick.GetAxis("Vertical", 0);
        }

        if(!_canShoot)
        {
            _shootTimer += Time.deltaTime;

            if(_shootTimer > 1f)
            {
                _shootTimer = 0f;
                _canShoot = true;
            }
        }

        //Debug.Log("H : " + horizontal + ", V : " + vertical);

        // Ignore movement of no inputs are recieved
        if (horizontal == 0f && vertical == 0f)
        {
            return;
        }

        if (_isMoving) return;
        
        if (horizontal != 0)
        {
            if (horizontal < 0)
            {
                horizontal = -1f;
            }
            else
            {
                horizontal = 1f;
            }
        }
        else
        {
            if (vertical < 0)
            {
                vertical = -1f;
            }
            else
            {
                vertical = 1f;
            }
        }

        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

        // Ignore camera Y axis so player doesn't move upward
        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        move = Vector3.zero;

        // Choose dominant axis to lock to 4 directions
        if (Mathf.Abs(vertical) > Mathf.Abs(horizontal))
        {
            move = camForward * Mathf.Sign(vertical); // North or South
        }
        else if (Mathf.Abs(horizontal) > 0)
        {
            move = camRight * Mathf.Sign(horizontal); // East or West
        }

        // check if a wall is in front
        Vector3 origin = _transform.position; 
        bool wallAhead = Physics.Raycast(origin, move.normalized, _wallCheckDistance, _obstacleLayer);

        // PLay animation of jump but dont move if wall is ahead
        if (!wallAhead)
        {
            _isMoving = true;
            _moveTimer = 0f;
            _animator.SetTrigger("jump");
        }

        // Rotate player to face movement
        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, rot, 10000000f * Time.deltaTime);
        }
    }

    public void ShootFireball()
    {
        if (!_canShoot) return;

        PoolController.Instance.SpawnFromPool("HopmonFireball", _fireballAimTransform.position, _fireballAimTransform.rotation);
        _canShoot = false;
        _shootTimer = 0f;
    }

    public void StackCrystal(Transform crystalTransform)
    {
        crystalTransform.position = _crystalStackTransform.position + _crystalStackOffset;
        crystalTransform.rotation = _crystalStackTransform.rotation;
        crystalTransform.SetParent(_crystalStackTransform);

        _crystalStackOffset += new Vector3(0f, _crystalStackYOffset, 0f);
    }

    public void ResetPlayer()
    {

    }

    // Round up decimal upto mentioned decimal point
    private float RoundUpToDecimal(float number, int numDecimalPlaces)
    {
        double multiplier = Math.Pow(10, numDecimalPlaces);
        // Multiply the number, round up using Mathf.Ceil, then divide back
        float result = Mathf.Ceil((float)(number * multiplier)) / (float)multiplier;
        return (Mathf.Round(result * 2f) / 2f);
    }
}
