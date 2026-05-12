using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Terresquall;
using System;
using System.Linq;

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

    public Transform BodyTransform
    {
        get { return _bodyTransform; }
    }
    [SerializeField] private Transform _bodyTransform;

    private Vector3 _lastMoveDirection = Vector3.zero;

    private List<Transform> _crystalTransformList;
    private List<CrystalView> _crystalViewList;

    public Transform ActiveFlyingCrystalTransform
    {
        get { return _activeFlyingCrystalTransform; }
    }
    private Transform _activeFlyingCrystalTransform;

    public Transform PlayerTransform
    {
        get { return _transform; }
    }
    private Transform _transform;
    private Animator _animator;
    private bool _isMoving;
    private bool _isInCooldown;
    private bool _canShoot;
    private bool _canFlyCrystal;
    private bool _isLevelEnding;

    public bool IsDead
    {
        get { return _isDead; }
    }
    private bool _isDead;
    private bool _shouldFixInputGlitch;
    private bool _canBoost;

    private float _moveTimer;
    private float _coolDownTimer;
    private float _shootTimer;
    private float _maxMoveTime;
    private float horizontal;
    private float vertical;
    private float _flyCrystalTimer;
    private float _shouldFixInputTimer;
    private float _originalMoveSpeed;

    private Vector3 _crystalStackOffset;
    private Vector3 move;
    private Vector3 boostMoveDirection;

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
        _crystalTransformList = new List<Transform>();
        _crystalViewList = new List<CrystalView>();
        _activeFlyingCrystalTransform = null;
        _originalMoveSpeed = _speed;
    }

    void Update()
    {
        if (_isDead) return;

        if(_shouldFixInputGlitch)
        {
            _shouldFixInputTimer += Time.deltaTime;

            if(_shouldFixInputTimer > 0.5f)
            {
                _shouldFixInputTimer = 0;
                _shouldFixInputGlitch = false;
            }
        }

        // PLayer is Moving
        if (_isMoving)
        {
            if(CameraController.Instance.FreeLookCam.LookAt != null)
            {
                CameraController.Instance.SetLookAtParameter(null);
            }

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

                if (_speed > _originalMoveSpeed && !_canBoost)
                {
                    SetMoveSpeed();
                    _animator.speed = _speed - 0.5f;
                    _animator.SetTrigger("idle");
                    _animator.ResetTrigger("slide");

                    Debug.Log("idle called");
                }

                if (_canBoost)
                {
                    _canBoost = false;
                    _speed = _originalMoveSpeed + 5f;
                    _maxMoveTime = 1f / _speed;
                    _animator.speed = _speed - 0.5f;
                    _isMoving = true;
                    move = boostMoveDirection;
                    _isInCooldown = false;
                    _animator.SetTrigger("slide");

                    if (move != Vector3.zero)
                    {
                        Quaternion rot = Quaternion.LookRotation(move);
                        _transform.rotation = Quaternion.Slerp(_transform.rotation, rot, 10000000f * Time.deltaTime);
                    }

                    return;
                }

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

        if(_canFlyCrystal && !_isLevelEnding)
        {
            if (_crystalTransformList.Count > 0 || LevelPrefabController.Instance.TotalCrystals <= 0)
            {
                _flyCrystalTimer += Time.deltaTime;

                if (_flyCrystalTimer > 1f)
                {
                    // If all crystals have been released
                    if(_crystalTransformList.Count <= 0)
                    {
                        _isLevelEnding = true;
                        _animator.SetTrigger("fly");
                        CameraController.Instance.StartLevelEndCameraMovement();
                        GameplayController.Instance.SetGameState(GameplayController.GameState.LevelEnd);
                    }
                    else
                    {
                        _activeFlyingCrystalTransform = _crystalTransformList.LastOrDefault();
                        _crystalTransformList.Remove(_activeFlyingCrystalTransform);
                        _crystalStackOffset -= new Vector3(0f, _crystalStackYOffset, 0f);

                        LevelPrefabController.Instance.ReleaseCrystal();
                        SetMoveSpeed();
                    }

                    _flyCrystalTimer = 0f;
                }
            }
        }

        // Take inputs if player is not moving, in cooldown, camera is not rotating or in pause mode
        if(!_shouldFixInputGlitch && !_isDead && !_isMoving && !_isInCooldown && !CameraController.Instance.IsMoving && GameplayController.Instance.CurrentGameState != GameplayController.GameState.Pause)
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
            _maxMoveTime = 1f / _speed;
            _animator.speed = _speed - 0.5f;
            _animator.SetTrigger("jump");
        }

        // Rotate player to face movement
        if (move != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(move);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, rot, 10000000f * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy") && !_isDead)
        {
            OnPlayerDeath();
        }

        if(other.CompareTag("Boost") && !_canBoost)
        {
            _canBoost = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Star") && !_canFlyCrystal)
        {
            _canFlyCrystal = true;
            _flyCrystalTimer = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Star") && _canFlyCrystal)
        {
            _canFlyCrystal = false;
            _flyCrystalTimer = 0f;
        }
    }

    public void SetBoostMoveDirection(Vector3 boostDirection)
    {
        if (boostDirection == boostMoveDirection) return;

        Debug.Log("Boost Move Direction : " + boostMoveDirection);

        boostMoveDirection = boostDirection;
    }

    public void OnPlayerDeath()
    {
        _isDead = true;

        for (int i = 0; i < _crystalViewList.Count; i++)
        {
            _crystalViewList[i].Scatter();
        }

        _animator.speed = _originalMoveSpeed - 0.5f;
        _animator.SetTrigger("dead");
        CameraController.Instance.StartLevelEndCameraMovement(true);
    }

    public void OnLevelEndAnimationFinish()
    {
        _isLevelEnding = false;

        if(_isDead)
        {
            UserInterfaceController.Instance.SetActiveUI(UserInterfaceController.UIState.MainMenu);
            return;
        }

        UserInterfaceController.Instance.SetActiveUI(UserInterfaceController.UIState.Gameplay);
    }

    public void PlayExplosionParticle()
    {
        PoolController.Instance.SpawnFromPool("Explosion", _transform.position, Quaternion.identity);
    }

    public void ShootFireball()
    {
        if (!_canShoot) return;

        PoolController.Instance.SpawnFromPool("HopmonFireball", _fireballAimTransform.position, _fireballAimTransform.rotation);
        _canShoot = false;
        _shootTimer = 0f;
    }

    public void StackCrystal(Transform crystalTransform, CrystalView crystalView)
    {
        crystalTransform.position = _crystalStackTransform.position + _crystalStackOffset;
        crystalTransform.rotation = _crystalStackTransform.rotation;
        crystalTransform.SetParent(_crystalStackTransform);
        _crystalTransformList.Add(crystalTransform);

        _crystalStackOffset += new Vector3(0f, _crystalStackYOffset, 0f);

        _crystalViewList.Add(crystalView);

        SetMoveSpeed();
    }

    public void SetMoveSpeed()
    {
        Debug.Log("Crystal Count : " + _crystalTransformList.Count);

        if (_crystalTransformList.Count >= 8 && _speed != (_originalMoveSpeed - 1f))
        {
            _speed = _originalMoveSpeed - 1;
            return;
        }

        if ((_crystalTransformList.Count >= 4 && _crystalTransformList.Count < 8) && _speed != (_originalMoveSpeed - 0.5f))
        {
            _speed = _originalMoveSpeed - 0.5f;
            return;
        }

        if (_crystalTransformList.Count < 4 && _speed != _originalMoveSpeed)
        {
            _speed = _originalMoveSpeed;
            return;
        }
    }

    public void ResetPlayer()
    {
        _crystalTransformList.Clear();
        _crystalViewList.Clear();
        _crystalStackOffset = Vector3.zero;
        _isLevelEnding = false;
        _animator.Rebind();
        _isDead = false;
        _isMoving = false;
        _canFlyCrystal = false;
        _speed = _originalMoveSpeed;

        _shouldFixInputGlitch = true;
        _shouldFixInputTimer = 0f;

        _canBoost = false;

        horizontal = 0f;
        vertical = 0f;

        // Reset Rotation
        Vector3 rot = _transform.eulerAngles;
        rot.y = -180;
        _transform.eulerAngles = rot;
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
