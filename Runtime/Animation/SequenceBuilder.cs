using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PrimeTween;
using Cysharp.Threading.Tasks;

namespace Spyke.Extras.Animation
{
    /// <summary>
    /// Fluent builder for creating PrimeTween animation sequences.
    /// </summary>
    public class SequenceBuilder
    {
        private readonly List<Action<Sequence>> _actions = new();
        private Action _onComplete;
        private int _cycles = 1;
        private CycleMode _cycleMode = CycleMode.Restart;

        /// <summary>
        /// Creates a new sequence builder.
        /// </summary>
        public static SequenceBuilder Create() => new();

        /// <summary>
        /// Adds a tween to chain after previous.
        /// </summary>
        public SequenceBuilder Chain(Tween tween)
        {
            _actions.Add(seq => seq.Chain(tween));
            return this;
        }

        /// <summary>
        /// Adds a tween to play alongside previous.
        /// </summary>
        public SequenceBuilder Group(Tween tween)
        {
            _actions.Add(seq => seq.Group(tween));
            return this;
        }

        /// <summary>
        /// Adds a nested sequence to chain after previous.
        /// </summary>
        public SequenceBuilder Chain(Sequence sequence)
        {
            _actions.Add(seq => seq.Chain(sequence));
            return this;
        }

        /// <summary>
        /// Adds a nested sequence to play alongside previous.
        /// </summary>
        public SequenceBuilder Group(Sequence sequence)
        {
            _actions.Add(seq => seq.Group(sequence));
            return this;
        }

        /// <summary>
        /// Adds a delay to chain.
        /// </summary>
        public SequenceBuilder Delay(float duration)
        {
            _actions.Add(seq => seq.ChainDelay(duration));
            return this;
        }

        /// <summary>
        /// Adds an action callback to chain.
        /// </summary>
        public SequenceBuilder Callback(Action action)
        {
            _actions.Add(seq => seq.ChainCallback(action));
            return this;
        }

        #region Common Animations

        /// <summary>
        /// Chains a fade in animation.
        /// </summary>
        public SequenceBuilder FadeIn(CanvasGroup canvasGroup, float duration = 0.3f)
        {
            return Chain(AnimationHelper.FadeIn(canvasGroup, duration));
        }

        /// <summary>
        /// Chains a fade out animation.
        /// </summary>
        public SequenceBuilder FadeOut(CanvasGroup canvasGroup, float duration = 0.3f)
        {
            return Chain(AnimationHelper.FadeOut(canvasGroup, duration));
        }

        /// <summary>
        /// Chains a scale in animation.
        /// </summary>
        public SequenceBuilder ScaleIn(Transform transform, float duration = 0.3f)
        {
            return Chain(AnimationHelper.ScaleIn(transform, duration));
        }

        /// <summary>
        /// Chains a scale out animation.
        /// </summary>
        public SequenceBuilder ScaleOut(Transform transform, float duration = 0.2f)
        {
            return Chain(AnimationHelper.ScaleOut(transform, duration));
        }

        /// <summary>
        /// Chains a scale pop animation.
        /// </summary>
        public SequenceBuilder ScalePop(Transform transform, float scale = 1.2f, float duration = 0.3f)
        {
            return Chain(AnimationHelper.ScalePop(transform, scale, duration));
        }

        /// <summary>
        /// Chains a slide in animation.
        /// </summary>
        public SequenceBuilder SlideIn(RectTransform rect, SlideDirection direction, float distance = 100f, float duration = 0.3f)
        {
            return Chain(AnimationHelper.SlideIn(rect, direction, distance, duration));
        }

        /// <summary>
        /// Chains a slide out animation.
        /// </summary>
        public SequenceBuilder SlideOut(RectTransform rect, SlideDirection direction, float distance = 100f, float duration = 0.3f)
        {
            return Chain(AnimationHelper.SlideOut(rect, direction, distance, duration));
        }

        /// <summary>
        /// Chains a shake animation.
        /// </summary>
        public SequenceBuilder Shake(Transform transform, float intensity = 10f, float duration = 0.5f)
        {
            return Chain(AnimationHelper.Shake(transform, intensity, duration));
        }

        /// <summary>
        /// Chains a bounce animation.
        /// </summary>
        public SequenceBuilder Bounce(Transform transform, float intensity = 0.1f, float duration = 0.4f)
        {
            return Chain(AnimationHelper.Bounce(transform, intensity, duration));
        }

        #endregion

        /// <summary>
        /// Sets the number of cycles.
        /// </summary>
        public SequenceBuilder WithCycles(int cycles, CycleMode mode = CycleMode.Restart)
        {
            _cycles = cycles;
            _cycleMode = mode;
            return this;
        }

        /// <summary>
        /// Loops infinitely.
        /// </summary>
        public SequenceBuilder Loop()
        {
            _cycles = -1;
            return this;
        }

        /// <summary>
        /// Sets a callback for when the sequence completes.
        /// </summary>
        public SequenceBuilder OnComplete(Action onComplete)
        {
            _onComplete = onComplete;
            return this;
        }

        /// <summary>
        /// Builds and returns the sequence.
        /// </summary>
        public Sequence Build()
        {
            var sequence = Sequence.Create(_cycles, _cycleMode);

            foreach (var action in _actions)
            {
                action(sequence);
            }

            if (_onComplete != null)
            {
                sequence.OnComplete(_onComplete);
            }

            return sequence;
        }

        /// <summary>
        /// Builds and plays the sequence.
        /// </summary>
        public Sequence Play()
        {
            return Build();
        }

        /// <summary>
        /// Builds, plays, and awaits the sequence.
        /// </summary>
        public async UniTask PlayAsync()
        {
            var sequence = Build();
            await sequence.ToUniTask();
        }
    }
}
