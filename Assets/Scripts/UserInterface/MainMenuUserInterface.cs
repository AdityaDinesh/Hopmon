using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUserInterface : UserInterface
{
    private GameObject _uiGameObject;

    private void Awake()
    {
        _uiGameObject = gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Show()
    {
        base.Show();
        _uiGameObject.SetActive(true);

        //Activate Main Menu Camera

    }

    public override void Hide()
    {
        base.Hide();
        _uiGameObject.SetActive(false);
    }
}
