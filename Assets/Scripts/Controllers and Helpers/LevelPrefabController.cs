using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LevelPrefabController : MonoBehaviour
{
    public static LevelPrefabController Instance;

    
    [SerializeField] private LevelData _levelData;
    [SerializeField] private string[] _levelAddressableKeys; // e.g. "Level_01", "Level_02"

    private int _currentLevel;
    private GameObject _currentLevelGameObject;

    private Dictionary<int, GameObject> _cachedLevelPrefabs = new Dictionary<int, GameObject>();
    private Dictionary<int, AsyncOperationHandle<GameObject>> _cachedHandles = new Dictionary<int, AsyncOperationHandle<GameObject>>();

    public bool IsReady { get; private set; } = false;

    public int TotalCrystals => _totalCrystals;
    private int _totalCrystals;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        _levelData.unlockedLevels = PlayerPrefs.GetInt("unlockedLevels");
        if (_levelData.unlockedLevels <= 0) _levelData.unlockedLevels = 1;
    }

    public IEnumerator PreloadAllLevels(System.Action<float> onProgress = null)
    {
        IsReady = false;

        for (int i = 0; i < _levelAddressableKeys.Length; i++)
        {
            if (_cachedLevelPrefabs.ContainsKey(i))
            {
                onProgress?.Invoke((float)(i + 1) / _levelAddressableKeys.Length);
                continue;
            }

            string key = _levelAddressableKeys[i];

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError($"[Level] Key at index {i} is empty. Check Inspector.");
                continue;
            }

            var handle = Addressables.LoadAssetAsync<GameObject>(key);
            yield return handle;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _cachedLevelPrefabs[i] = handle.Result;
                _cachedHandles[i] = handle;
            }
            else
            {
                Debug.LogError($"[Level] Failed to preload '{key}': {handle.OperationException}");
            }

            onProgress?.Invoke((float)(i + 1) / _levelAddressableKeys.Length);
            yield return null;
        }

        IsReady = true;
        Debug.Log("[Level] All levels preloaded.");
    }

    public void SetLevelData(int levelNumber)
    {
        _currentLevel = levelNumber;

        if (!_cachedLevelPrefabs.TryGetValue(_currentLevel, out GameObject prefab))
        {
            Debug.LogError($"[Level] Level {_currentLevel} not preloaded yet.");
            return;
        }

        _currentLevelGameObject = Instantiate(prefab, UserInterfaceController.Instance.LevelLoadPosition.position, UserInterfaceController.Instance.LevelLoadPosition.rotation);

        LevelPrefabData levelPrefabData = _currentLevelGameObject.GetComponent<LevelPrefabData>();
        if (levelPrefabData != null)
            levelPrefabData.LoadLevelData();
    }

    public void LoadNextLevel()
    {
        _currentLevel++;

        if (_currentLevelGameObject != null)
            Destroy(_currentLevelGameObject);

        if (_currentLevel >= _levelData.totalLevels) return;

        if (!_cachedLevelPrefabs.TryGetValue(_currentLevel, out GameObject prefab))
        {
            Debug.LogError($"[Level] Level {_currentLevel} not preloaded yet.");
            return;
        }

        _currentLevelGameObject = Instantiate(prefab, UserInterfaceController.Instance.LevelLoadPosition.position, UserInterfaceController.Instance.LevelLoadPosition.rotation);

        LevelPrefabData levelPrefabData = _currentLevelGameObject.GetComponent<LevelPrefabData>();
        if (levelPrefabData != null)
            levelPrefabData.LoadLevelData();

        if (_currentLevel >= _levelData.unlockedLevels)
        {
            _levelData.unlockedLevels++;
            PlayerPrefs.SetInt("unlockedLevels", _levelData.unlockedLevels);
            Debug.Log("Unlocked Level Count : " + _levelData.unlockedLevels);
        }
    }

    public void HideCurrentLevel()
    {
        if (_currentLevelGameObject == null) return;
        Destroy(_currentLevelGameObject);
        _currentLevelGameObject = null;
    }

    public void SetLevelUI(int crystalNumber)
    {
        UserInterfaceController.Instance.UpdateLevelUI(_currentLevel.ToString("00"), false);
        UserInterfaceController.Instance.UpdateLevelUI(crystalNumber.ToString("00"));
        _totalCrystals = crystalNumber;
    }

    public void ReleaseCrystal()
    {
        _totalCrystals--;
        UserInterfaceController.Instance.UpdateLevelUI(_totalCrystals.ToString("00"));
    }

    public bool IsThisLastLevel()
    {
        Debug.Log("Current Level : " + _currentLevel + ", Total Levels : " + _levelData.totalLevels);
        return _currentLevel >= (_levelData.totalLevels - 1);
    }

    private void OnDestroy()
    {
        foreach (var kvp in _cachedHandles)
        {
            if (kvp.Value.IsValid())
                Addressables.Release(kvp.Value);
        }

        _cachedHandles.Clear();
        _cachedLevelPrefabs.Clear();
    }
}