using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterface : MonoBehaviour
{
    private GameObject _gameObject;

    private void Awake()
    {
        _gameObject = gameObject;
    }

    public virtual void Show()
    {
        _gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        _gameObject.SetActive(false);
    }
}
