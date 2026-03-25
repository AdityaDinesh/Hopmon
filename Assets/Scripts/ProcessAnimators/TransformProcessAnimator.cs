#region Namespaces

using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     The TransformProcessAnimator is a Sequence Animator class that can house multiple animation processes the execute on the Transform component of the Object.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Transform))]
public class TransformProcessAnimator : SerializedMonoBehaviour
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    [System.Flags]
    private enum AnimationFlags
    {
        RepeatAnimation = 1 << 0,
        AnimateOnStart = 1 << 1,
        AnimateWhilePaused = 1 << 2,
    }

    public enum AnimatorTypes
    {
        Position_World,
        Scale_Punch,
        Scale_Single
    }

    [BoxGroup("Animator Configuration")]
    [BoxGroup("Animator Configuration/Settings")]
    [EnumToggleButtons, HideLabel]
    [SerializeField] private AnimationFlags _configurationFlags;
    public bool AnimateOnPause
    {
        get { return _configurationFlags.HasFlag(AnimationFlags.AnimateWhilePaused); }
    }

    [BoxGroup("Animator Configuration/Sequence")]
    [HideReferenceObjectPicker]
    [ListDrawerSettings(HideAddButton = true)]
    [SerializeField] private List<TransformProcess> _animatorSequence = new List<TransformProcess>();
    public TransformProcess AnimatorSequence
    {
        get => _animatorSequence[0];
    }

    private int _animatorIndex;
    public Transform Transform { get; private set; }

    private Vector3 _sourcePosition;
    private Vector3 _sourceRotation;
    private Vector3 _sourceScale;

    private bool _animating;

    private Coroutine _animationCoroutine;

    #endregion // Variables.

    // ########################################
    // MonoBehaviour Methods.
    // ########################################

    #region MonoBehaviour

    private void Awake()
    {
        Transform = GetComponent<Transform>();

        _sourcePosition = Transform.position;
        _sourceScale = Transform.localScale;
        _sourceRotation = Transform.rotation.eulerAngles;
    }

    private void Start()
    {
        if (_configurationFlags.HasFlag(AnimationFlags.AnimateOnStart))
        {
            StartAnimation();
        }
    }

    #endregion // MonoBehaviour Methods.

    // ########################################
    // Methods.
    // ########################################

    #region Methods

    /// <summary>
    ///     Adds an Animator to the Transform Animator Sequence.
    /// </summary>
    /// <param name="animatorType"> The Animator Type to add. </param>
    [BoxGroup("Animator Configuration/Sequence")]
    [Button("Add Animator")]
    public void AddAnimatorToSequence(AnimatorTypes animatorType)
    {
        switch (animatorType)
        {
            case AnimatorTypes.Position_World:
                _animatorSequence.Add(new TransformWorldPositionProcess());
                break;
            case AnimatorTypes.Scale_Punch:
                _animatorSequence.Add(new TransformPunchScaleProcess());
                break;
            case AnimatorTypes.Scale_Single:
                _animatorSequence.Add(new TransformSingleScaleProcess());
                break;
            default:
                throw new UnassignedReferenceException();
        }
    }

    /// <summary>
    ///     Start the animation of the scale.
    /// </summary>
    [DisableInEditorMode]
    [ButtonGroup("Animator Configuration/Settings/Tools")]
    [Button("Start Animation")]
    public void StartAnimation()
    {
        if (_animating) return;

        _animatorIndex = 0;
        _animatorSequence[_animatorIndex].Setup(this);
        _animationCoroutine = StartCoroutine(_animatorSequence[_animatorIndex].Animation());
        _animating = true;
    }

    /// <summary>
    ///     Stop the animation of the scale.
    /// </summary>
    [DisableInEditorMode]
    [ButtonGroup("Animator Configuration/Settings/Tools")]
    [Button("Stop Animation")]
    public void StopAnimation()
    {
        if (_animationCoroutine == null) return;

        StopCoroutine(_animationCoroutine);
        _animatorSequence[_animatorIndex].KillSequence();
        _animating = false;
    }

    /// <summary>
    ///     Resets the Animation Sequence.
    /// </summary>
    [DisableInEditorMode]
    [ButtonGroup("Animator Configuration/Settings/Tools")]
    [Button("Reset Animation")]
    public void ResetAnimation()
    {
        if (_animating)
        {
            StopAnimation();
        }

        _animatorIndex = 0;

        Transform.position = _sourcePosition;
        Transform.localScale = _sourceScale;
        Transform.rotation = Quaternion.Euler(_sourceRotation);
    }

    /// <summary>
    ///     Moves the Animator to the next Animation in Sequence.
    /// </summary>
    public void NextAnimation()
    {
        if (!_animating) return;

        if (_animatorIndex + 1 > _animatorSequence.Count - 1)
        {
            if (_configurationFlags.HasFlag(AnimationFlags.RepeatAnimation))
            {
                _animatorIndex = 0;
                _animatorSequence[_animatorIndex].Setup(this);
                _animationCoroutine = StartCoroutine(_animatorSequence[_animatorIndex].Animation());
            }
            else
            {
                StopAnimation();
                return;
            }
        }
        else
        {
            _animatorIndex++;

            _animatorSequence[_animatorIndex].Setup(this);
            _animationCoroutine = StartCoroutine(_animatorSequence[_animatorIndex].Animation());
        }
    }

    public void RemoveAllAnimators()
    {
        _animatorSequence.Clear();
    }

    #endregion // Methods.
}
