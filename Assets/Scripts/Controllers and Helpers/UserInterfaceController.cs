using System.Collections;
using System.Collections.Generic;
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

        _userInterfaceDictionary = new Dictionary<UIState, UserInterface>();

        for (int i = 0; i < userInterfaceTypesList.Count; i++)
        {
            _userInterfaceDictionary.Add(userInterfaceTypesList[i].uIState, userInterfaceTypesList[i].UserInterface);
        }
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
        _currentUIState = uiState;
        _loadingScreenAnimator.SetTrigger("show");
        Debug.Log(_currentUIState);
    }

    public void SetActiveUIFromEditor(int uiStateID)
    {
        _currentUIState = (UIState)uiStateID;
        _loadingScreenAnimator.SetTrigger("show");
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
            //mainMenuUserInterface.Show();
            //_activeUserInterface = _userInterfaceDictionary[_currentUIState];
            //_loadingScreenAnimator.SetTrigger("hide");
            //return;
        }

        _activeUserInterface = _userInterfaceDictionary[_currentUIState];
        _userInterfaceDictionary[_currentUIState].Show();
        _loadingScreenAnimator.SetTrigger("hide");
    }

    public void HideAllUI()
    {
        foreach (var userInterface in _userInterfaceDictionary.Values)
        {
            userInterface.Hide();
        }
    }
}
