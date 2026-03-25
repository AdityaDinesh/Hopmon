#region Namespaces

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     This Transform Process moves the Object to a series of waypoints.
/// </summary>
public class TransformWorldPositionProcess : TransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    [BoxGroup("Transform World Position Sequence")]
    [SerializeField] private List<WorldPositionData> _worldPositionData = new List<WorldPositionData>();

    #endregion // Variables.

    // ########################################
    // Methods.
    // ########################################

    #region Methods

    /// <summary>
    ///     Set up the Transform Sequence
    /// </summary>
    /// <param name="sequenceAnimator"> A reference to the Sequence Animator controlling it. </param>
    public override void Setup(TransformProcessAnimator sequenceAnimator)
    {
        base.Setup(sequenceAnimator);
    }

    /// <summary>
    ///     The Animation's Sequence.
    /// </summary>
    public override IEnumerator Animation()
    {

        _animationSequence.Kill();
        _animationSequence = DOTween.Sequence();

        float totalDuration = 0f;

        for (int i = 0; i < _worldPositionData.Count; i++)
        {
            WorldPositionData positionData = _worldPositionData[i];

            if (i == 0)
            {
                totalDuration += positionData.DelayUntilStart;
                _animationSequence.AppendInterval(positionData.DelayUntilStart);
                _animationSequence.Append(_transform.DOMove(positionData.Position.localPosition, positionData.Duration).SetEase(positionData.EaseType)).SetUpdate(_animateOnPause);
            }
            else
            {
                // Count up duration of previous point + start delay of current point.
                totalDuration += _worldPositionData[i - 1].Duration + positionData.DelayUntilStart;
                _animationSequence.AppendInterval(positionData.DelayUntilStart);
                _animationSequence.Append(_transform.DOMove(positionData.Position.localPosition, positionData.Duration).SetEase(positionData.EaseType)).SetUpdate(_animateOnPause);
            }
        }

        // Get the last duration of the final point.
        totalDuration += _worldPositionData[_worldPositionData.Count - 1].Duration;

        if (_animateOnPause)
        {
            yield return new WaitForSecondsRealtime(totalDuration);
        }
        else
        {
            yield return new WaitForSeconds(totalDuration);
        }


        _sequenceAnimator.NextAnimation();
    }

    #endregion // Methods.
}


public class WorldPositionData
{
    public Transform Position;
    public float DelayUntilStart;
    public float Duration = 1f;
    public Ease EaseType = Ease.Linear;
}