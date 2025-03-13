using System.Runtime.CompilerServices;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryItemDropEffect : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private MotionHandle _scaleMotionHandle = default;
    private MotionHandle _alphaMotionHandle = default;
    [SerializeField] private float ScaleInDuration = 0.2f;
    [SerializeField] private float ScaleOutDuration = 0.3f;
    [SerializeField] private float FadeInDuration = 0.1f;
    [SerializeField] private float FadeOutDuration = 0.1f;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ResetAnimations()
    {
        _rectTransform.localScale = Vector3.one * 0.2f;
        _canvasGroup.alpha = 0.8f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CancelAllAnimations()
    {
        if (_scaleMotionHandle.IsActive()) _scaleMotionHandle.Cancel();
        if (_alphaMotionHandle.IsActive()) _alphaMotionHandle.Cancel();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AnimateScaleIn()
    {
        _scaleMotionHandle = LMotion.Create(_rectTransform.localScale, Vector3.one, ScaleInDuration)
            .WithEase(Ease.OutBack)
            .BindToLocalScale(_rectTransform);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AnimateScaleOut()
    {
        _scaleMotionHandle = LMotion.Create(_rectTransform.localScale, Vector3.one * 0.2f, ScaleOutDuration)
            .WithEase(Ease.InQuad)
            .BindToLocalScale(_rectTransform);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AnimateFadeIn()
    {
        _alphaMotionHandle = LMotion.Create(_canvasGroup.alpha, 1f, FadeInDuration)
            .WithEase(Ease.OutQuad)
            .BindToAlpha(_canvasGroup);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AnimateFadeOut()
    {
        _alphaMotionHandle = LMotion.Create(_canvasGroup.alpha, 0.8f, FadeOutDuration)
            .WithEase(Ease.InQuad)
            .BindToAlpha(_canvasGroup);
    }
}
