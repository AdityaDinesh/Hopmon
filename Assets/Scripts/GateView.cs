using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateView : MonoBehaviour
{
    private Transform _transform;
    private Animator _Animator;

    private bool _isDestroyed;

    private void Awake()
    {
        _transform = transform;

        _Animator = GetComponent<Animator>();
    }

    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Fireball"))
        {
            _isDestroyed = true;
            _Animator.SetTrigger("destroy");
        }
    }

}
