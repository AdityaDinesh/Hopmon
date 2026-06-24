using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private string _gameSceneName;

    public void Start()
    {
        LoadScene(_gameSceneName);
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadLevelAsync(sceneName));
    }

    IEnumerator LoadLevelAsync(string sceneName)
    {
        // Phase 1: Preload level assets (0% - 50%)
        yield return StartCoroutine(LevelPrefabController.Instance.PreloadAllLevels(
            progress => _loadingSlider.value = Mathf.Lerp(0f, 0.5f, progress)
        ));

        // Phase 2: Load game scene (50% - 100%)
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName);
        loadOperation.allowSceneActivation = false;

        while (loadOperation.progress < 0.9f)
        {
            float progressValue = Mathf.Lerp(0.5f, 1f, loadOperation.progress / 0.9f);
            _loadingSlider.value = progressValue;
            yield return null;
        }

        _loadingSlider.value = 1f;
        yield return new WaitForSeconds(0.2f); // brief pause so slider visually hits 100%

        loadOperation.allowSceneActivation = true;
    }
}
