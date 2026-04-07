using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPrefabData : MonoBehaviour
{
    [SerializeField] private Transform _playerSpawn;

    //UI Data

    [SerializeField] private int _totalCrystals;

    // Start is called before the first frame update
    void Start()
    {
    }

    public void LoadLevelData()
    {
        PlayerController.Instance.PlayerTransform.position = _playerSpawn.position;

        // Set UI values

        LevelPrefabController.Instance.SetLevelUI(_totalCrystals);
    }

    public void UnloadLevelData()
    {
        gameObject.SetActive(false);

        // Set UI values
    }
}
