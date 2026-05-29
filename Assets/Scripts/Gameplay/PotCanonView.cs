using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotCanonView : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private float _shootRate = 2f;

    private Transform _transform;

    private float _shootTimer;

    private void Awake()
    {
        _shootTimer = 0f;
        _transform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameplayController.Instance.CurrentGameState == GameplayController.GameState.Menu) return;

        _shootTimer += Time.deltaTime;

        if(_shootTimer > _shootRate)
        {
            _shootTimer = 0f;

            // Spawn Fireball

            Quaternion rotation = Quaternion.LookRotation(_transform.up);

            PoolController.Instance.SpawnFromPool("PotCanonFireball", spawnPoint.position, rotation);
            AudioController.Instance.PlaySFX(SfxSoundType.PotCanonFireball, _transform.position);
        }
    }

    //public void ShootFireballs()
    //{
    //    float angleStep = 360f / fireballCount;

    //    for (int i = 0; i < fireballCount; i++)
    //    {
    //        float angle = i * angleStep;
    //        float rad = angle * Mathf.Deg2Rad;

    //        Vector3 direction = new Vector3(
    //            Mathf.Cos(rad),
    //            0f,
    //            Mathf.Sin(rad)
    //        ).normalized;

    //        // Exact landing position
    //        Vector3 targetPosition =
    //            spawnPoint.position + direction * landingRadius;

    //        Quaternion rotation =
    //            Quaternion.LookRotation(direction);

    //        GameObject obj = fireballPool.Spawn(
    //            spawnPoint.position,
    //            rotation
    //        );

    //        Fireball fireball = obj.GetComponent<Fireball>();

    //        fireball.Setup(
    //            spawnPoint.position,
    //            targetPosition,
    //            arcHeight
    //        );
    //    }
    //}

    
}
