using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController Instance;

    public enum GameState
    {
        Start,
        Playing,
        Pause,
        GameOver
    }

    [SerializeField] private LevelData _levelData;
    [SerializeField] private GameObject[] _levelPrefabList;
    [SerializeField] private Transform _levelLoadPositionTransform;

    private GameState _currentGameState = GameState.Start;
    private int _currentlevel = 0;
    private GameObject _currentLevelGameObject;

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
        _currentGameState = GameState.Start;
        CameraController.Instance.SetCamera(CameraController.CameraType.MainMenu);
        UserInterfaceController.Instance.HideAllUI();
        UserInterfaceController.Instance.SetActiveUI(UserInterfaceController.UIState.MainMenu);
        UserInterfaceController.Instance.ShowUI();
    }

    public void SetGameState(GameState gameState)
    {
        if (_currentGameState == gameState) return;

        _currentGameState = gameState;

        if(_currentGameState == GameState.Playing)
        {
            
        }

        if(_currentGameState == GameState.GameOver)
        {
            
        }
    }

    public void StartGame(int levelNumber)
    {
        _currentlevel = levelNumber;
        _currentLevelGameObject = Instantiate(_levelPrefabList[_currentlevel], _levelLoadPositionTransform.position, _levelLoadPositionTransform.rotation);

        PlayerController.Instance.gameObject.SetActive(true);
        PlayerController.Instance.ResetPlayer();
        CameraController.Instance.SetCamera(CameraController.CameraType.GamePlay);
    }

    public void LoadNextLevel()
    {
        _currentlevel++;
        Destroy(_currentLevelGameObject);
        _currentLevelGameObject = Instantiate(_levelPrefabList[_currentlevel], _levelLoadPositionTransform.position, _levelLoadPositionTransform.rotation);
    }

    public void GameOver()
    {
        // Do Stuff For Game Reset

        UserInterfaceController.Instance.SetActiveUI(UserInterfaceController.UIState.MainMenu);
        PlayerController.Instance.gameObject.SetActive(false);
    }
}
