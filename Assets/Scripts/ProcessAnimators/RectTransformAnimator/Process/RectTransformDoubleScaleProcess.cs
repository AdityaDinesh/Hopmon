#region Namespaces

using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     This Rect Transform Process that scales the Rect Transform of the Object twice before returning it to the base scale.
/// </summary>
public class RectTransformDoubleScaleProcess : RectTransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    private Vector3 _sourceScale;
    [BoxGroup("Double Scale Sequence")]
    [SerializeField] private Vector3 _animationFirstScale = new Vector3(1.04f, 1.04f, 1.04f);
    [BoxGroup("Double Scale Sequence")]
    [SerializeField] private Vector3 _animationSecondScale = new Vector3(1.08f, 1.08f, 1.08f);

    [BoxGroup("Double Scale Sequence")]
    [SerializeField] private Ease _ease = Ease.OutSine;

    [BoxGroup("Double Scale Sequence")]
    [SerializeField] private float _startDelay = 0f;
    [BoxGroup("Double Scale Sequence")]
    [SerializeField] private float _firstScaleDuration = 0.2f;
    [BoxGroup("Double Scale Sequence")]
    [SerializeField] private float _secondScaleDuration = 0.2f;
    [BoxGroup("Double Scale Sequence")]
    [SerializeField] private float _nextAnimationDelay = 0.8f;

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

        _animationSequence.Append(_rectTransform.DOScale(_animationFirstScale, _firstScaleDuration)).SetEase(_ease).SetUpdate(_animateOnPause);
        _animationSequence.Append(_rectTransform.DOScale(_sourceScale, _firstScaleDuration)).SetEase(_ease).SetUpdate(_animateOnPause);
        _animationSequence.Append(_rectTransform.DOScale(_animationSecondScale, _secondScaleDuration)).SetEase(_ease).SetUpdate(_animateOnPause);
        _animationSequence.Append(_rectTransform.DOScale(_sourceScale, _secondScaleDuration)).SetEase(_ease).SetUpdate(_animateOnPause);

        if (_animateOnPause)
        {
            yield return new WaitForSecondsRealtime(_firstScaleDuration + _firstScaleDuration + _secondScaleDuration + _secondScaleDuration + 0.05f + _nextAnimationDelay);
        }
        else
        {
            yield return new WaitForSecondsRealtime(_firstScaleDuration + _firstScaleDuration + _secondScaleDuration + _secondScaleDuration + 0.05f + _nextAnimationDelay);
        }

        _sequenceAnimator.NextAnimation();
    }

    #endregion // Methods.
}
