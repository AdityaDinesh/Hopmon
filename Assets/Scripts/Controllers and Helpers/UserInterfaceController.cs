using System.Collections;
using System.Collections.Generic;
using Terresquall;
using TMPro;
using UnityEngine;

[System.Serializable]
public class UserInterfaceType
{
    public UserInterfaceController.UIState uIState;
    public UserInterface UserInterface;
}

public class UserInterfaceController : MonoBehaviour
{
    public static UserInterfaceController Instance;

    public enum UIState
    {
        MainMenu = 0,
        LevelSelect = 1,
        Gameplay = 2,
        Pause = 3,
    }

    private Dictionary<UIState, UserInterface> _userInterfaceDictionary;

    [SerializeField] private List<UserInterfaceType> userInterfaceTypesList;

    

    public MainMenuUserInterface CurrentMainMenuUserInterface
    {
        get { return mainMenuUserInterface; }
    }
    [SerializeField] private MainMenuUserInterface mainMenuUserInterface;

    public Animator LoadingScreenAnimator
    {
        get { return _loadingScreenAnimator; }
    }
    [SerializeField] private Animator _loadingScreenAnimator;

    public UserInterface ActiveUserInterface
    {
        get { return _activeUserInterface; }
    }
    private UserInterface _activeUserInterface;

    public UIState CurrentUIState
    {
        get { return _currentUIState; }
    }
    private UIState _currentUIState = UIState.MainMenu;

    [SerializeField] private GameObject _inputControlsParentGameObject;
    [SerializeField] private GameObject _pauseUIGameObject;

    [SerializeField] private GameObject _debugPanelGameObject;
    [SerializeField] private TextMeshProUGUI _debugText;
    [SerializeField] private bool _displayDebugText;

    [Header("Level Controller Data")]
    [SerializeField] private TextMeshProUGUI _levelNumberText;
    [SerializeField] private TextMeshProUGUI _crystalNumberText;

    public Transform LevelLoadPosition
    {
        get { return _levelLoadPositionTransform; }
    }
    [SerializeField] private Transform _levelLoadPositionTransform;

    private UIState _previousUIState;
    private bool _isPaused;

    private void Awake()
    {
        Debug.Log($"AWAKE_START: UIController {System.DateTime.Now:HH:mm:ss.fff}");

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

        _userInterfaceDictionary = new Dictionary<UIState, UserInterface>();

        for (int i = 0; i < userInterfaceTypesList.Count; i++)
        {
            _userInterfaceDictionary.Add(userInterfaceTypesList[i].uIState, userInterfaceTypesList[i].UserInterface);
        }

        if(!_displayDebugText)
        {
            _debugPanelGameObject.SetActive(false);
        }

        Debug.Log($"AWAKE_END: UIController {System.DateTime.Now:HH:mm:ss.fff}");
    }

    // Start is called before the first frame update
    //void Start()
    //{
    //    HideAllUI();
    //    SetActiveUI(UIState.MainMenu);
    //    ShowUI();
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public void SetActiveUI(UIState uiState)
    {
        _previousUIState = _currentUIState;

        _currentUIState = uiState;
        _loadingScreenAnimator.SetTrigger("show");
        Debug.Log(_currentUIState);
    }

    public void SetActiveUIFromEditor(int uiStateID)
    {
        _currentUIState = (UIState)uiStateID;
        _loadingScreenAnimator.SetTrigger("show");
        AudioController.Instance.PlaySFX(SfxSoundType.UISelect); //
    }

    public void ShowUI()
    {
        if(_activeUserInterface != null)
        {
            _activeUserInterface.Hide();
        }

        if(_currentUIState == UIState.MainMenu)
        {
            _loadingScreenAnimator.ResetTrigger("show");

            //if(GameplayController.Instance.CurrentGameState == GameplayController.GameState.GameOver)
            //{
            //    PlayerController.Instance.gameObject.SetActive(false);
            //    LevelPrefabController.Instance.HideCurrentLevel();
            //}

            //mainMenuUserInterface.Show();
            //_activeUserInterface = _userInterfaceDictionary[_currentUIState];
            //_loadingScreenAnimator.SetTrigger("hide");
            //return;
        }

        if (_currentUIState == UIState.MainMenu && _previousUIState == UIState.Gameplay)
        {
            GameplayController.Instance.GameOver();
        }

        // Load new level once UI loading canvas has started show animation
        if (_currentUIState == UIState.Gameplay && _previousUIState == UIState.Gameplay)
        {
            LevelPrefabController.Instance.LoadNextLevel();
            PlayerController.Instance.ResetPlayer();
            CameraController.Instance.FinishLevelEndCameraMovement();
            CameraController.Instance.SetCamera(CameraController.CameraType.GamePlay);
            GameplayController.Instance.SetGameState(GameplayController.GameState.Playing);
        }

        _activeUserInterface = _userInterfaceDictionary[_currentUIState];
        _userInterfaceDictionary[_currentUIState].Show();
        _loadingScreenAnimator.SetTrigger("hide");
    }

    public void PauseGame()
    {
        if (_isPaused) return;

        _pauseUIGameObject.SetActive(true);
        Time.timeScale = 0f;
        _isPaused = true;
        GameplayController.Instance.SetGameState(GameplayController.GameState.Pause);
        SetInputUI(true);
        AudioController.Instance.PlaySFX(SfxSoundType.UISelect);
    }

    public void ResumeGame()
    {
        if (!_isPaused) return;

        _pauseUIGameObject.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
        GameplayController.Instance.SetGameState(GameplayController.GameState.Playing);
        SetInputUI(false);
        AudioController.Instance.PlaySFX(SfxSoundType.UISelect);
    }

    public void EndGame()
    {
        // What happens when you hit home on pause menu.
        // Trigger Player kill -> Game Over -> Main Menu
        
        _pauseUIGameObject.SetActive(false);
        Time.timeScale = 1f;
        _isPaused = false;
        PlayerController.Instance.OnPlayerDeath();
    }

    public void HideAllUI()
    {
        foreach (var userInterface in _userInterfaceDictionary.Values)
        {
            userInterface.Hide();
        }
    }

    public void SetInputUI(bool hide)
    {
        if(_inputControlsParentGameObject == null)
        {
            Debug.LogError("Input Control ParentGameObject is Null");
            return;
        }

        if(hide == true && _inputControlsParentGameObject.activeInHierarchy)
        {
            _inputControlsParentGameObject.SetActive(false);
            return;
        }

        if (hide == false && !_inputControlsParentGameObject.activeInHierarchy)
        {
            _inputControlsParentGameObject.SetActive(true);
        }
    }

    public void UpdateLevelUI(string text, bool isCrystalData = true)
    {
        if(isCrystalData)
        {
            _crystalNumberText.text = text;
            return;
        }

        _levelNumberText.text = text;
    }

    public void ShowDebugText(string text)
    {
        if(text != null && _debugText != null)
        {
            _debugText.text = text;
        }
    }
}
