using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
    public static GameplayController Instance;

    public enum GameState
    {
        Menu,
        Playing,
        Pause,
        LevelEnd
    }

    public GameState CurrentGameState
    {
        get { return _currentGameState; }
    }
    private GameState _currentGameState = GameState.Menu;

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
        AudioController.Instance.PlayMusic(BgmSoundType.MainMenu);
        SetGameState(GameState.Menu);
        CameraController.Instance.SetCamera(CameraController.CameraType.MainMenu);
        UserInterfaceController.Instance.HideAllUI();
        UserInterfaceController.Instance.SetActiveUI(UserInterfaceController.UIState.MainMenu);
        UserInterfaceController.Instance.ShowUI();
    }

    public void SetGameState(GameState gameState)
    {
        if (_currentGameState == gameState) return;

        _currentGameState = gameState;

        if (_currentGameState == GameState.Menu)
        {
            AudioController.Instance.PlayMusic(BgmSoundType.MainMenu);
        }

        if (_currentGameState == GameState.Playing)
        {
            AudioController.Instance.PlayMusic(BgmSoundType.World1);
        }

        if (_currentGameState == GameState.LevelEnd)
        {
            
        }
    }

    public void StartGame(int levelNumber)
    {
        PlayerController.Instance.gameObject.SetActive(true);
        PlayerController.Instance.ResetPlayer();
        CameraController.Instance.SetCamera(CameraController.CameraType.GamePlay);
        LevelPrefabController.Instance.SetLevelData(levelNumber);
        SetGameState(GameState.Playing);
    }

    public void GameOver()
    {
        // Do Stuff For Game Reset

        PlayerController.Instance.gameObject.SetActive(false);
        CameraController.Instance.FinishLevelEndCameraMovement();
        CameraController.Instance.SetCamera(CameraController.CameraType.MainMenu);
        LevelPrefabController.Instance.HideCurrentLevel();
        //UserInterfaceController.Instance.SetActiveUI(UserInterfaceController.UIState.MainMenu);
        _currentGameState = GameState.Menu;
    }
}
