#region Namespaces

using UnityEngine;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     This Rect Transform Process that rotates the Rect Transform at a given rate before returning it to its root rotation.
/// </summary>
public class RectTransformFullRotationProcess : RectTransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    public enum RotateDirectionType
    {
        Forwards,
        Backwards
    }

    public enum RotateAxisType
    {
        X,
        Y,
        Z
    }

    private Vector3 _sourceRotation;

    [BoxGroup("Full Rotation Sequence")]
    [SerializeField] private RotateDirectionType _rotateDirectionType = RotateDirectionType.Forwards;
    [BoxGroup("Full Rotation Sequence")]
    [SerializeField] private RotateAxisType _rotateAxisType = RotateAxisType.Z;
    [BoxGroup("Full Rotation Sequence")]
    [SerializeField] private Ease _ease = Ease.Linear;

    [BoxGroup("Full Rotation Sequence")]
    [SerializeField] private float _startDelay = 0f;
    [BoxGroup("Full Rotation Sequence")]
    [SerializeField] private float _duration = 4f;
    [BoxGroup("Full Rotation Sequence")]
    [SerializeField] private float _nextAnimationDelay = 0f;

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

        float degrees = 360f;
        if (_rotateDirectionType == RotateDirectionType.Forwards)
        {
            degrees = -360f;
        }
        else
        {
            degrees = 360f;
        }

        Vector3 rotationVector3 = new Vector3(0f, 0f, 0f);
        if (_rotateAxisType == RotateAxisType.X)
        {
            rotationVector3 = new Vector3(degrees, 0f, 0f);
        }
        else if (_rotateAxisType == RotateAxisType.Y)
        {
            rotationVector3 = new Vector3(0f, degrees, 0f);
        }
        else if (_rotateAxisType == RotateAxisType.Z)
        {
            rotationVector3 = new Vector3(0f, 0f, degrees);
        }

        _animationSequence.Kill();
        _animationSequence = DOTween.Sequence();

        _animationSequence.Append(_rectTransform.DORotate(rotationVector3, _duration, RotateMode.FastBeyond360).SetEase(_ease)).SetUpdate(_animateOnPause);

        if (_animateOnPause)
        {
            yield return new WaitForSecondsRealtime(_duration + _nextAnimationDelay);
        }
        else
        {
            yield return new WaitForSeconds(_duration + _nextAnimationDelay);
        }

        _sequenceAnimator.NextAnimation();
    }

    #endregion // Methods.
}
