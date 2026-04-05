using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUserInterface : UserInterface
{
    [SerializeField] private GameObject _mainMenuPlayerGameObject;
    [SerializeField] private Transform _mainMenuPlayerTransform;
    [SerializeField] private Vector3 _mainMenuPlayerRotationSpeed;

    private GameObject _uiGameObject;

    private void Awake()
    {
        _uiGameObject = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(_mainMenuPlayerGameObject.activeInHierarchy)
        {
            _mainMenuPlayerTransform.Rotate(_mainMenuPlayerRotationSpeed * Time.deltaTime);
        }
    }

    public override void Show()
    {
        //base.Show();
        _uiGameObject.SetActive(true);
        _mainMenuPlayerGameObject.SetActive(true);
        _mainMenuPlayerTransform.rotation = Quaternion.identity;
        PlayerController.Instance.gameObject.SetActive(false);
    }

    public override void Hide()
    {
        base.Hide();
        _mainMenuPlayerGameObject.SetActive(false);
    }
}
