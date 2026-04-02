using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectUserInterface : UserInterface
{
    [SerializeField] private GameObject _levelPreviewGameObject;
    [SerializeField] private Transform _levelRotatorTransform;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _tiltAngle = 25f;
    private Transform _cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        _cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!_levelPreviewGameObject.activeInHierarchy) return;

        _levelRotatorTransform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime, Space.Self);
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
    }
}
