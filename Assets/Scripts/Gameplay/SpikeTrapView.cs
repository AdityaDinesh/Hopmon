using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapView : MonoBehaviour
{
    [SerializeField] private float fallDistance = 5f;
    [SerializeField] private Transform _bodyTransform;

    [Header("Speeds")]
    [SerializeField] private float fallSpeed = 20f;
    [SerializeField] private float riseSpeed = 3f;

    [Header("Timers")]
    [SerializeField] private float waitBeforeFall = 2f;
    [SerializeField] private float waitOnGround = 1f;

    private Vector3 startPos;
    private Vector3 bottomPos;

    private float timer;

    private enum TrapState
    {
        WaitingTop,
        Falling,
        WaitingBottom,
        Rising
    }

    private TrapState currentState;


    // Start is called before the first frame update
    void Start()
    {
        startPos = _bodyTransform.position;
        bottomPos = startPos + Vector3.down * fallDistance;

        currentState = TrapState.WaitingTop;
        timer = waitBeforeFall;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TrapState.WaitingTop:
                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    currentState = TrapState.Falling;
                }
                break;

            case TrapState.Falling:

                _bodyTransform.position = Vector3.MoveTowards(
                    _bodyTransform.position,
                    bottomPos,
                    fallSpeed * Time.deltaTime
                );

                if (Vector3.Distance(_bodyTransform.position, bottomPos) < 0.01f)
                {
                    _bodyTransform.position = bottomPos;

                    currentState = TrapState.WaitingBottom;
                    timer = waitOnGround;
                }

                break;

            case TrapState.WaitingBottom:

                timer -= Time.deltaTime;

                if (timer <= 0f)
                {
                    currentState = TrapState.Rising;
                }

                break;

            case TrapState.Rising:

                _bodyTransform.position = Vector3.MoveTowards(
                    _bodyTransform.position,
                    startPos,
                    riseSpeed * Time.deltaTime
                );

                if (Vector3.Distance(_bodyTransform.position, startPos) < 0.01f)
                {
                    _bodyTransform.position = startPos;

                    currentState = TrapState.WaitingTop;
                    timer = waitBeforeFall;
                }

                break;
        }
    }
}
