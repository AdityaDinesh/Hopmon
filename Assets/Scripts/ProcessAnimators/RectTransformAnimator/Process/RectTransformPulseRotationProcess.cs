#region Namespaces

using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     This Rect Transform Process that rotates the Rect Transfrom by a given degree before returning it to its base rotation.
/// </summary>
public class RectTransformPulseRotationProcess : RectTransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    private Vector3 _sourceRotation;
    [BoxGroup("Pulse Rotation Sequence")]
    [SerializeField] private Vector3 _animationRotation = new Vector3(0f, 0f, 35f);

    [BoxGroup("Pulse Rotation Sequence")]
    [SerializeField] private Ease _ease = Ease.OutSine;

    [BoxGroup("Pulse Rotation Sequence")]
    [SerializeField] private float _startDelay = 0f;
    [BoxGroup("Pulse Rotation Sequence")]
    [SerializeField] private float _duration = 0.4f;
    [BoxGroup("Pulse Rotation Sequence")]
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

        _sourceRotation = _rectTransform.rotation.eulerAngles;
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

        _animationSequence.Append(_rectTransform.DORotate(new Vector3(_animationRotation.x, _animationRotation.y, _animationRotation.z), _duration).SetEase(_ease)).SetUpdate(_animateOnPause);
        _animationSequence.Append(_rectTransform.DORotate(new Vector3(_sourceRotation.x, _sourceRotation.y, _sourceRotation.z), _duration).SetEase(_ease)).SetUpdate(_animateOnPause);

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
