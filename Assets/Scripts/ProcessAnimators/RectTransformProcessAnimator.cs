#region Namespaces

using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     The RectTransformProcessAnimator is a Process Animator class that can house multiple animation processes that execute on the associated object's Rect Transform.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class RectTransformProcessAnimator : SerializedMonoBehaviour
{
    // ########################################
    // Enums.
    // ########################################

    #region Enums

    [System.Flags]
    private enum AnimationFlags
    {
        RepeatAnimation = 1 << 0,
        AnimateOnStart = 1 << 1,
        AnimateWhilePaused = 1 << 2,
    }

    public enum AnimatorTypes
    {
        Position_Anchored,
        Rotation_Pulse,
        Rotation_Full,
        Scale_Single,
        Scale_Double,
        Scale_Punch,
        Size_Delta
    }

    #endregion // Enums.

    // ########################################
    // Variables.
    // ########################################

    #region Variables

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
    [SerializeField] private List<RectTransformProcess> _animatorSequence = new List<RectTransformProcess>();

    private int _animatorIndex;
    public RectTransform RectTransform { get; private set; }

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
        RectTransform = GetComponent<RectTransform>();

        _sourcePosition = RectTransform.anchoredPosition;
        _sourceScale = RectTransform.localScale;
        _sourceRotation = RectTransform.rotation.eulerAngles;
    }

    private void Start()
    {
        if (_configurationFlags.HasFlag(AnimationFlags.AnimateOnStart))
        {
            StartAnimation();
        }
    }

    private void OnEnable()
    {
        ResetAnimation();
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
    ///     Adds an Animator to the RectTransform Animator Sequence.
    /// </summary>
    /// <param name="animatorType"> The Animator Type to add. </param>
    [BoxGroup("Animator Configuration/Sequence")]
    [Button("Add Animator")]
    public void AddAnimatorToSequence(AnimatorTypes animatorType)
    {
        switch (animatorType)
        {
            case AnimatorTypes.Position_Anchored:
                _animatorSequence.Add(new RectTransformAnchoredPositionProcess());
                break;
            case AnimatorTypes.Rotation_Pulse:
                _animatorSequence.Add(new RectTransformPulseRotationProcess());
                break;
            case AnimatorTypes.Rotation_Full:
                _animatorSequence.Add(new RectTransformFullRotationProcess());
                break;
            case AnimatorTypes.Scale_Single:
                _animatorSequence.Add(new RectTransformSingleScaleProcess());
                break;
            case AnimatorTypes.Scale_Double:
                _animatorSequence.Add(new RectTransformDoubleScaleProcess());
                break;
            case AnimatorTypes.Scale_Punch:
                _animatorSequence.Add(new RectTransformPunchScaleProcess());
                break;
            case AnimatorTypes.Size_Delta:
                _animatorSequence.Add(new RectTransformSizeDeltaProcess());
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

        RectTransform.anchoredPosition = _sourcePosition;
        RectTransform.localScale = _sourceScale;
        RectTransform.rotation = Quaternion.Euler(_sourceRotation);
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

    #endregion // Methods.
}