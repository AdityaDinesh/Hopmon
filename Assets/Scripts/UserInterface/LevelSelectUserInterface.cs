using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUserInterface : UserInterface
{
    [SerializeField] private GameObject _levelPreviewGameObject;
    [SerializeField] private Transform _levelRotatorTransform;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _tiltAngle = 25f;

    [SerializeField] private LevelData levelData;

    private int currentLevel = 0;
    private string _activelevelTitle;
    private bool _canStartLevel;

    [SerializeField] private GameObject[] levelPreviews;
    [SerializeField] private TextMeshProUGUI levelText;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    // Start is called before the first frame update
    void Start()
    {
        UpdateLevel();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!_levelPreviewGameObject.activeInHierarchy) return;

        _levelRotatorTransform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.Self);
    }
    public void NextLevel()
    {
        Debug.Log("Level Next");

        if (currentLevel < levelData.unlockedLevels - 1)
        {
            currentLevel++;
            UpdateLevel();
        }
    }
    public void PreviousLevel()
    {
        if (currentLevel > 0)
        {
            currentLevel--;
            UpdateLevel();
        }
    }

    void UpdateLevel()
    {

        for (int i = 0; i < levelPreviews.Length; i++)
        {
            levelPreviews[i].SetActive(i == currentLevel);
        }

        levelText.text = "LEVEL " + (currentLevel + 1).ToString("00");

        leftButton.interactable = currentLevel > 0;
        rightButton.interactable = currentLevel < levelData.unlockedLevels - 1;
    }

    public void LoadSelectedLevel()
    {
        _canStartLevel = true;
        UserInterfaceController.Instance.SetActiveUI(UserInterfaceController.UIState.Gameplay);
    }

    public override void Show()
    {
        base.Show();
        _levelPreviewGameObject.SetActive(true);
    }

    public override void Hide()
    {
        base.Hide();
        _levelPreviewGameObject.SetActive(false);

        if (!_canStartLevel) return;

        GameplayController.Instance.StartGame(currentLevel);
    }


}
