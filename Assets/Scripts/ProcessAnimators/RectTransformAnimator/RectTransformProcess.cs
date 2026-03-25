#region Namespaces

using UnityEngine;
using System.Collections;
using DG.Tweening;

#endregion // Namespaces.

/// <summary>
///     Rect Transform Process is an abstract class from which all Rect Transform Processes inherit.
/// </summary>
[System.Serializable]
public abstract class RectTransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    protected RectTransformProcessAnimator _sequenceAnimator;
    protected RectTransform _rectTransform
    {
        get { return _sequenceAnimator.RectTransform; }
    }
    protected bool _animateOnPause
    {
        get { return _sequenceAnimator.AnimateOnPause; }
    }

    protected Sequence _animationSequence;

    #endregion // Variables.

    // ########################################
    // Methods.
    // ########################################

    #region Methods

    /// <summary>
    ///     Set up the Rect Transform Sequence
    /// </summary>
    /// <param name="sequenceAnimator"> A reference to the Sequence Animator controlling it. </param>
    public virtual void Setup(RectTransformProcessAnimator sequenceAnimator)
    {
        _sequenceAnimator = sequenceAnimator;
    }

    /// <summary>
    ///     Kill the current Sequence.
    /// </summary>
    public void KillSequence()
    {
        _animationSequence.Kill();
        _animationSequence = DOTween.Sequence();
    }

    /// <summary>
    ///     The Animation's Sequence.
    /// </summary>
    public abstract IEnumerator Animation();

    #endregion // Methods.
}
