#region Namespaces

using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     This Rect Transform Process that punches the scale of the Rect Transform of the Object before returning it to the base scale.
/// </summary>
public class RectTransformPunchScaleProcess : RectTransformProcess
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
    ///     Set up the Rect Transform Sequence
    /// </summary>
    /// <param name="sequenceAnimator"> A reference to the Sequence Animator controlling it. </param>
    public override void Setup(RectTransformProcessAnimator sequenceAnimator)
    {
        base.Setup(sequenceAnimator);

        _sourceScale = _rectTransform.localScale;
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

        _animationSequence.Append(_rectTransform.DOPunchScale(_animationPunchScale, _duration, _animationVibrato, _animationElasticity).SetEase(_ease)).SetUpdate(_animateOnPause);

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

    #endregion // Methods.
}
