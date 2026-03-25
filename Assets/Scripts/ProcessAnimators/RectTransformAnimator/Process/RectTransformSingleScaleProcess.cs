#region Namespaces

using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     This Rect Transform Process that scales the Rect Transform of the Object before returning it to the base scale.
/// </summary>
public class RectTransformSingleScaleProcess : RectTransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    private Vector3 _sourceScale;
    [BoxGroup("Single Scale Sequence")]
    [SerializeField] private Vector2 _animationScale = new Vector2(1.04f, 1.04f);

    [BoxGroup("Single Scale Sequence")]
    [SerializeField] private Ease _ease = Ease.OutSine;

    [BoxGroup("Single Scale Sequence")]
    [SerializeField] private float _startDelay = 0f;
    [BoxGroup("Single Scale Sequence")]
    [SerializeField] private float _duration = 0.4f;
    [BoxGroup("Single Scale Sequence")]
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

        _animationSequence.Append(_rectTransform.DOScale(new Vector3(_animationScale.x, _animationScale.y, _sourceScale.z), _duration).SetEase(_ease)).SetUpdate(_animateOnPause);
        _animationSequence.Append(_rectTransform.DOScale(new Vector3(_sourceScale.x, _sourceScale.y, _sourceScale.z), _duration).SetEase(_ease)).SetUpdate(_animateOnPause);

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
