#region Namespaces

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;

#endregion // Namespaces.

/// <summary>
///     This Rect Transform Process that moves the Rect Transform to a given series of waypoints.
/// </summary>
public class RectTransformAnchoredPositionProcess : RectTransformProcess
{
    // ########################################
    // Variables.
    // ########################################

    #region Variables

    [BoxGroup("Anchored Position Sequence")]
    [HideReferenceObjectPicker]
    [SerializeField] private List<AnchoredPositionData> _anchoredPositionData = new List<AnchoredPositionData>();

    #endregion // Variables.

    // ########################################
    // Methods.
    // ########################################

    #region Methods

    /// <summary>
    /// Sets the sprite and color of the image at the end of the animation.
    /// </summary>
    /// <param name="image">The image to update</param>
    /// <param name="sprite">The chosen Sprite</param>
    /// <param name="color">the chosen Colour</param>
    private void AnimationOnComplete(Image image, Sprite sprite, Color color)
    {
        image.sprite = sprite;
        image.color = color;
    }

    #endregion // Methods.

    // ########################################
    // Coroutines.
    // ########################################

    #region Coroutines

    /// <summary>
    ///     The Animation's Sequence.
    /// </summary>
    public override IEnumerator Animation()
    {
        _animationSequence.Kill();
        _animationSequence = DOTween.Sequence();

        float totalDuration = 0f;

        for (int i = 0; i < _anchoredPositionData.Count; i++)
        {
            AnchoredPositionData positionData = _anchoredPositionData[i];

            if (i == 0)
            {
                totalDuration += positionData.DelayUntilStart;
                _animationSequence.AppendInterval(positionData.DelayUntilStart);
                // Scale is dummy code for the OnComplete call.
                if (positionData.ChangeImageOnStart)
                {
                    _animationSequence.Append(_rectTransform.DOScale(_rectTransform.localScale, 0f).SetUpdate(_animateOnPause).OnComplete(() => AnimationOnComplete(positionData.Image, positionData.SpriteOnStart, positionData.ColorOnStart)));
                }
                _animationSequence.Append(_rectTransform.DOAnchorPos(positionData.Position.anchoredPosition, positionData.Duration).SetEase(positionData.EaseType)).SetUpdate(_animateOnPause);
            }
            else
            {
                // Count up duration of previous point + start delay of current point.
                totalDuration += _anchoredPositionData[i - 1].Duration + positionData.DelayUntilStart;
                _animationSequence.AppendInterval(positionData.DelayUntilStart);
                // Scale is dummy code for the OnComplete call.
                if (positionData.ChangeImageOnStart)
                {
                    _animationSequence.Append(_rectTransform.DOScale(_rectTransform.localScale, 0f).SetUpdate(_animateOnPause).OnComplete(() => AnimationOnComplete(positionData.Image, positionData.SpriteOnStart, positionData.ColorOnStart)));
                }
                _animationSequence.Append(_rectTransform.DOAnchorPos(positionData.Position.anchoredPosition, positionData.Duration).SetEase(positionData.EaseType)).SetUpdate(_animateOnPause);
            }
        }

        // Get the last duration of the final point.
        totalDuration += _anchoredPositionData[_anchoredPositionData.Count - 1].Duration;

        if (_animateOnPause)
        {
            yield return new WaitForSecondsRealtime(totalDuration);
        }
        else
        {
            yield return new WaitForSeconds(totalDuration);
        }

        _sequenceAnimator.NextAnimation();
    }

    #endregion // Coroutines.
}

/// <summary>
///     The Position Data for the Waypoints
/// </summary>
[System.Serializable]
public class AnchoredPositionData
{
    [BoxGroup("Position Data")]
    public RectTransform Position;
    [BoxGroup("Position Data")]
    public float DelayUntilStart;
    [BoxGroup("Position Data")]
    public float Duration = 1f;
    [BoxGroup("Position Data")]
    public Ease EaseType = Ease.Linear;
    [BoxGroup("Position Data")]
    public bool ChangeImageOnStart = false;
    [BoxGroup("Position Data")]
    [ShowIf("ChangeImageOnStart")]
    public Image Image;
    [BoxGroup("Position Data")]
    [ShowIf("ChangeImageOnStart")]
    public Sprite SpriteOnStart;
    [BoxGroup("Position Data")]
    [ShowIf("ChangeImageOnStart")]
    public Color ColorOnStart = Color.white;
}
