using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Loads/Hides Level Prefab Data and also updates UI level elements
/// </summary>

public class LevelPrefabController : MonoBehaviour
{
    public static LevelPrefabController Instance;

    [SerializeField] private TextMeshProUGUI _levelNumberText; 
    [SerializeField] private TextMeshProUGUI _crystalNumberText;

    [SerializeField] private LevelData _levelData;
    [SerializeField] private GameObject[] _levelPrefabList;
    [SerializeField] private Transform _levelLoadPositionTransform;

    private int _currentLevel;
    private GameObject _currentLevelGameObject;

    public int TotalCrystals
    {
        get { return _totalCrystals; }
    }
    private int _totalCrystals;

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

    public void SetLevelData(int levelNumber)
    {
        _currentLevel = levelNumber;
        _currentLevelGameObject = Instantiate(_levelPrefabList[_currentLevel], _levelLoadPositionTransform.position, _levelLoadPositionTransform.rotation);
        LevelPrefabData levelPrefabData = _currentLevelGameObject.GetComponent<LevelPrefabData>();

        if(levelPrefabData != null)
        {
            levelPrefabData.LoadLevelData();
        }
    }

    public void LoadNextLevel()
    {
        _currentLevel++;
        Destroy(_currentLevelGameObject);
        _currentLevelGameObject = Instantiate(_levelPrefabList[_currentLevel], _levelLoadPositionTransform.position, _levelLoadPositionTransform.rotation);
        LevelPrefabData levelPrefabData = _currentLevelGameObject.GetComponent<LevelPrefabData>();

        if (levelPrefabData != null)
        {
            levelPrefabData.LoadLevelData();
        }

        // Increment unlocked levels in level data scriptable object, but also ensure it doesn't exceed total number of levels
        _levelData.unlockedLevels = _levelData.unlockedLevels + 1 <= _levelData.totalLevels ? _levelData.unlockedLevels++ : _levelData.unlockedLevels; 
    }

    public void SetLevelUI(int crystalNumber)
    {
        _levelNumberText.text = _currentLevel.ToString("00");
        _crystalNumberText.text = crystalNumber.ToString("00");
        _totalCrystals = crystalNumber;
    }

    public void ReleaseCrystal()
    {
        _totalCrystals--;
        _crystalNumberText.text = _totalCrystals.ToString("00");
    }

    public void HideCurrentLevel()
    {
        if (_currentLevelGameObject == null) return;

        Destroy(_currentLevelGameObject);
    }
}
