using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using Cysharp.Threading.Tasks;

namespace Spyke.Extras.Animation
{
    /// <summary>
    /// Common animation patterns using PrimeTween.
    /// </summary>
    public static class AnimationHelper
    {
        #region Fade

        /// <summary>
        /// Fades a CanvasGroup in.
        /// </summary>
        public static Tween FadeIn(CanvasGroup canvasGroup, float duration = 0.3f, Ease ease = Ease.OutQuad)
        {
            canvasGroup.alpha = 0f;
            return Tween.Alpha(canvasGroup, 1f, duration, ease);
        }

        /// <summary>
        /// Fades a CanvasGroup out.
        /// </summary>
        public static Tween FadeOut(CanvasGroup canvasGroup, float duration = 0.3f, Ease ease = Ease.InQuad)
        {
            return Tween.Alpha(canvasGroup, 0f, duration, ease);
        }

        /// <summary>
        /// Fades an Image in.
        /// </summary>
        public static Tween FadeIn(Graphic graphic, float duration = 0.3f, Ease ease = Ease.OutQuad)
        {
            var color = graphic.color;
            color.a = 0f;
            graphic.color = color;
            return Tween.Alpha(graphic, 1f, duration, ease);
        }

        /// <summary>
        /// Fades an Image out.
        /// </summary>
        public static Tween FadeOut(Graphic graphic, float duration = 0.3f, Ease ease = Ease.InQuad)
        {
            return Tween.Alpha(graphic, 0f, duration, ease);
        }

        #endregion

        #region Scale

        /// <summary>
        /// Scale pop animation (enlarge then shrink).
        /// </summary>
        public static Sequence ScalePop(Transform transform, float scale = 1.2f, float duration = 0.3f)
        {
            var originalScale = transform.localScale;
            var targetScale = originalScale * scale;
            var halfDuration = duration * 0.5f;

            return Sequence.Create()
                .Chain(Tween.Scale(transform, targetScale, halfDuration, Ease.OutBack))
                .Chain(Tween.Scale(transform, originalScale, halfDuration, Ease.InOutQuad));
        }

        /// <summary>
        /// Scale in from zero.
        /// </summary>
        public static Tween ScaleIn(Transform transform, float duration = 0.3f, Ease ease = Ease.OutBack)
        {
            transform.localScale = Vector3.zero;
            return Tween.Scale(transform, Vector3.one, duration, ease);
        }

        /// <summary>
        /// Scale out to zero.
        /// </summary>
        public static Tween ScaleOut(Transform transform, float duration = 0.2f, Ease ease = Ease.InBack)
        {
            return Tween.Scale(transform, Vector3.zero, duration, ease);
        }

        /// <summary>
        /// Bounce animation on scale.
        /// </summary>
        public static Sequence Bounce(Transform transform, float intensity = 0.1f, float duration = 0.4f)
        {
            var originalScale = transform.localScale;
            var quarterDuration = duration * 0.25f;

            return Sequence.Create()
                .Chain(Tween.Scale(transform, originalScale * (1f + intensity), quarterDuration, Ease.OutQuad))
                .Chain(Tween.Scale(transform, originalScale * (1f - intensity * 0.5f), quarterDuration, Ease.InOutQuad))
                .Chain(Tween.Scale(transform, originalScale * (1f + intensity * 0.25f), quarterDuration, Ease.InOutQuad))
                .Chain(Tween.Scale(transform, originalScale, quarterDuration, Ease.InOutQuad));
        }

        #endregion

        #region Move

        /// <summary>
        /// Slide in from a direction.
        /// </summary>
        public static Tween SlideIn(RectTransform rect, SlideDirection direction, float distance = 100f, float duration = 0.3f, Ease ease = Ease.OutQuad)
        {
            var targetPos = rect.anchoredPosition;
            var startPos = targetPos + GetDirectionOffset(direction) * distance;
            rect.anchoredPosition = startPos;
            return Tween.UIAnchoredPosition(rect, targetPos, duration, ease);
        }

        /// <summary>
        /// Slide out to a direction.
        /// </summary>
        public static Tween SlideOut(RectTransform rect, SlideDirection direction, float distance = 100f, float duration = 0.3f, Ease ease = Ease.InQuad)
        {
            var targetPos = rect.anchoredPosition + GetDirectionOffset(direction) * distance;
            return Tween.UIAnchoredPosition(rect, targetPos, duration, ease);
        }

        private static Vector2 GetDirectionOffset(SlideDirection direction)
        {
            return direction switch
            {
                SlideDirection.Left => Vector2.left,
                SlideDirection.Right => Vector2.right,
                SlideDirection.Up => Vector2.up,
                SlideDirection.Down => Vector2.down,
                _ => Vector2.zero
            };
        }

        /// <summary>
        /// Shake animation.
        /// </summary>
        public static Tween Shake(Transform transform, float intensity = 10f, float duration = 0.5f)
        {
            return Tween.ShakeLocalPosition(transform, new Vector3(intensity, intensity, 0), duration);
        }

        #endregion

        #region Rotation

        /// <summary>
        /// Spin rotation.
        /// </summary>
        public static Tween Spin(Transform transform, float rotations = 1f, float duration = 1f, Ease ease = Ease.Linear)
        {
            var targetRotation = transform.localEulerAngles + Vector3.forward * 360f * rotations;
            return Tween.LocalRotation(transform, Quaternion.Euler(targetRotation), duration, ease);
        }

        /// <summary>
        /// Wobble rotation.
        /// </summary>
        public static Sequence Wobble(Transform transform, float angle = 15f, float duration = 0.5f)
        {
            var originalRotation = transform.localEulerAngles;
            var quarterDuration = duration * 0.25f;

            return Sequence.Create()
                .Chain(Tween.LocalRotation(transform, Quaternion.Euler(0, 0, angle), quarterDuration, Ease.OutQuad))
                .Chain(Tween.LocalRotation(transform, Quaternion.Euler(0, 0, -angle), quarterDuration * 2f, Ease.InOutQuad))
                .Chain(Tween.LocalRotation(transform, Quaternion.Euler(originalRotation), quarterDuration, Ease.InOutQuad));
        }

        #endregion

        #region Combined

        /// <summary>
        /// Show with scale and fade.
        /// </summary>
        public static Sequence ShowScaleFade(Transform transform, CanvasGroup canvasGroup, float duration = 0.3f)
        {
            transform.localScale = Vector3.zero;
            canvasGroup.alpha = 0f;

            return Sequence.Create()
                .Group(Tween.Scale(transform, Vector3.one, duration, Ease.OutBack))
                .Group(Tween.Alpha(canvasGroup, 1f, duration, Ease.OutQuad));
        }

        /// <summary>
        /// Hide with scale and fade.
        /// </summary>
        public static Sequence HideScaleFade(Transform transform, CanvasGroup canvasGroup, float duration = 0.2f)
        {
            return Sequence.Create()
                .Group(Tween.Scale(transform, Vector3.zero, duration, Ease.InBack))
                .Group(Tween.Alpha(canvasGroup, 0f, duration, Ease.InQuad));
        }

        /// <summary>
        /// Attention-grabbing animation (scale + shake).
        /// </summary>
        public static Sequence Attention(Transform transform)
        {
            return Sequence.Create()
                .Chain(ScalePop(transform, 1.15f, 0.2f))
                .Chain(Shake(transform, 5f, 0.2f));
        }

        #endregion
    }

    /// <summary>
    /// Direction for slide animations.
    /// </summary>
    public enum SlideDirection
    {
        Left,
        Right,
        Up,
        Down
    }
}
