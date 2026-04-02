using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    private GameObject _gameObject;


    public virtual void Show()
    {
        if (_gameObject == null) _gameObject = gameObject;

        _gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        if (_gameObject == null) _gameObject = gameObject;

        _gameObject.SetActive(false);
    }
}
