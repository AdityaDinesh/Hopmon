using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Terresquall;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public enum CameraType
    {
        MainMenu,
        GamePlay,
    }

    [SerializeField] private CinemachineFreeLook _freeLookCam;
    [SerializeField] private CinemachineFreeLook _mainMenuCam;
    [SerializeField] private float _rotationSpeed = 5f;

    private Transform _playerTransform;

    public bool IsMoving
    {
        get { return _isMoving; }
    }
    private bool _isMoving;
    private bool _isInMovingCooldown;

    private float _horizontal;

    private float _cooldownTimer;
    private float _targetAngle;

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
    }

    // Start is called before the first frame update
    void Start()
    {
        //_playerTransform = PlayerController.Instance.PlayerTransform;
    }

    // Update is called once per frame
    void Update()
    {
        //Rotate Camera by some speed over time

        if(_isMoving)
        {
            _freeLookCam.m_XAxis.Value = Mathf.MoveTowards(_freeLookCam.m_XAxis.Value, _targetAngle, _rotationSpeed * Time.deltaTime);

            if (Mathf.Approximately(_freeLookCam.m_XAxis.Value, _targetAngle))
            {
                _freeLookCam.m_XAxis.Value = _targetAngle;

                if (_freeLookCam.m_XAxis.Value == _freeLookCam.m_XAxis.m_MaxValue || _freeLookCam.m_XAxis.Value == _freeLookCam.m_XAxis.m_MinValue)
                {
                    _freeLookCam.m_XAxis.Value = 0f;
                }
                _isMoving = false;
                _isInMovingCooldown = true;
                _cooldownTimer = 0f;
                _freeLookCam.m_LookAt = null;
            }

            return;
        }

        //Cooldown after camera rotation

        if (_isInMovingCooldown)
        {
            _cooldownTimer += Time.deltaTime;

            if(_cooldownTimer >= 0.1f)
            {
                _isInMovingCooldown = false;
                _cooldownTimer = 0f;
            }

            return;
        }

        // Recieve input to calculate in which direction camera should rotate

        _horizontal = VirtualJoystick.GetAxis("Horizontal", 1);

        if (_horizontal == 0) return;
        _horizontal = -1 * Mathf.Sign(_horizontal);

        _isMoving = true;
        float xVal = _freeLookCam.m_XAxis.Value;

        _targetAngle = xVal + (_horizontal * 90f);

        _freeLookCam.m_LookAt = _playerTransform;
    }

    public void SetCamera(CameraType cameraType)
    {
        if(cameraType == CameraType.MainMenu)
        {
            _freeLookCam.Priority = 5;
            _mainMenuCam.Priority = 10;
            return;
        }

        if(_playerTransform == null)
        {
            _playerTransform = PlayerController.Instance.PlayerTransform;
        }

        _freeLookCam.Priority = 10;
        _mainMenuCam.Priority = 5;
    }
}
