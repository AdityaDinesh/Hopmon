#region Namespaces

using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     This Rect Transform Process that changes the size of the Rect Transform before returning it to its base size.
/// </summary>
public class RectTransformSizeDeltaProcess : RectTransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    private Vector2 _sourceSizeDelta;
    [BoxGroup("Size Delta Sequence")]
    [SerializeField] private Vector2 _animationSizeDelta = new Vector2(0f, 0f);

    [BoxGroup("Size Delta Sequence")]
    [SerializeField] private Ease _ease = Ease.OutSine;

    [BoxGroup("Size Delta Sequence")]
    [SerializeField] private float _startDelay = 0f;
    [BoxGroup("Size Delta Sequence")]
    [SerializeField] private float _duration = 0.4f;
    [BoxGroup("Size Delta Sequence")]
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

        _sourceSizeDelta = _rectTransform.sizeDelta;
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

        _animationSequence.Append(_rectTransform.DOSizeDelta(_animationSizeDelta, _duration).SetEase(_ease)).SetUpdate(_animateOnPause);
        _animationSequence.Append(_rectTransform.DOSizeDelta(_sourceSizeDelta, _duration).SetEase(_ease)).SetUpdate(_animateOnPause);

        if (_animateOnPause)
        {
            yield return new WaitForSecondsRealtime(_duration * 2 + _nextAnimationDelay);
        }
        else
        {
            yield return new WaitForSeconds(_duration * 2 + _nextAnimationDelay);
        }

        _sequenceAnimator.NextAnimation();
    }

    #endregion // Methods.
}
