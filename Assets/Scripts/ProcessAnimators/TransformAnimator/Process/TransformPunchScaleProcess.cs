#region Namespaces

using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     This Transform Process punches the scale to a given target before returning it to its base.
/// </summary>
public class TransformPunchScaleProcess : TransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    private Vector3 _sourceScale;
    [BoxGroup("Punch Scale Sequence")]
    [SerializeField] private Vector3 _animationPunchScale = new Vector3(0.05f, 0.05f, 0.05f);
    [BoxGroup("Punch Scale Sequence")]
    [Range(5, 20)]
    [SerializeField] private int _animationVibrato = 8;
    [BoxGroup("Punch Scale Sequence")]
    [Range(0f, 1f)]
    [SerializeField] private float _animationElasticity = 1f;

    [BoxGroup("Punch Scale Sequence")]
    [SerializeField] private Ease _ease = Ease.OutSine;

    [BoxGroup("Punch Scale Sequence")]
    [SerializeField] private float _startDelay = 0f;
    [BoxGroup("Punch Scale Sequence")]
    [SerializeField] private float _duration = 0.4f;
    [BoxGroup("Punch Scale Sequence")]
    [SerializeField] private float _nextAnimationDelay = 1f;

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

        _sourceScale = _transform.localScale;
    }

    /// <summary>
    ///     The Animation's Sequence.
    /// </summary>
    public override IEnumerator Animation()
    {
        if (_animateOnPause)
        {
            yield return new WaitForSecondsRealtime(_startDelay);
        }
        else
        {
            yield return new WaitForSeconds(_startDelay);
        }

        _animationSequence.Kill();
        _animationSequence = DOTween.Sequence();

        _animationSequence.Append(_transform.DOPunchScale(_animationPunchScale, _duration, _animationVibrato, _animationElasticity).SetEase(_ease)).SetUpdate(_animateOnPause);

        if (_animateOnPause)
        {
            yield return new WaitForSecondsRealtime(_duration + 0.05f + _nextAnimationDelay);
        }
        else
        {
            yield return new WaitForSeconds(_duration + 0.05f + _nextAnimationDelay);
        }

        _sequenceAnimator.NextAnimation();
    }

    //public override void ChangeSettings(EntityBehaviourData entityBehaviourData)
    //{
    //    _animationPunchScale = entityBehaviourData.PunchAnimationScale;
    //    _animationVibrato = entityBehaviourData.PunchAnimationVibrato;
    //    _animationElasticity = entityBehaviourData.PunchAnimationElasticity;
    //    _startDelay = entityBehaviourData.PunchStartDelay;
    //    _duration = entityBehaviourData.PunchDuration;
    //    _nextAnimationDelay = entityBehaviourData.PunchNextAnimationDelay;
    //}

    //public override void ChangeSettings(ObjectBehaviourData objectBehaviourData)
    //{
    //    _animationPunchScale = objectBehaviourData.PunchAnimationScale;
    //    _animationVibrato = objectBehaviourData.PunchAnimationVibrato;
    //    _animationElasticity = objectBehaviourData.PunchAnimationElasticity;
    //    _startDelay = objectBehaviourData.PunchStartDelay;
    //    _duration = objectBehaviourData.PunchDuration;
    //    _nextAnimationDelay = objectBehaviourData.PunchNextAnimationDelay;
    //}

    #endregion // Methods.
}
