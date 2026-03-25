#region Namespaces

using UnityEngine;
using System.Collections;
using DG.Tweening;

#endregion // Namespaces.

/// <summary>
///     Transform Process is an abstract class from which all Transform Processes inherit.
/// </summary>
[System.Serializable]
public abstract class TransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    protected TransformProcessAnimator _sequenceAnimator;
    protected Transform _transform
    {
        get { return _sequenceAnimator.Transform; }
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
    ///     Set up the Transform Sequence
    /// </summary>
    /// <param name="sequenceAnimator"> A reference to the Sequence Animator controlling it. </param>
    public virtual void Setup(TransformProcessAnimator sequenceAnimator)
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

    //public virtual void ChangeSettings(EntityBehaviourData entityBehaviourData) { }

    //public virtual void ChangeSettings(ObjectBehaviourData objectBehaviourData) { }

    #endregion // Methods.
}
