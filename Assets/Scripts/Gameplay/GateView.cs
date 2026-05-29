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
        
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Fireball"))
        {
            _isDestroyed = true;
            _Animator.SetTrigger("destroy");

            //PoolController.Instance.SpawnFromPool("Fireball", _transform.position, Quaternion.identity);
            AudioController.Instance.PlaySFX(SfxSoundType.FireballHit, _transform.position);
        }
    }
    public void PlayExplosionParticle()
    {
        PoolController.Instance.SpawnFromPool("Explosion", _transform.position, Quaternion.identity);
        AudioController.Instance.PlaySFX(SfxSoundType.Explosion, _transform.position);
    }

}
