using UnityEngine;
using PrimeTween;

namespace Spyke.Extras.Effects
{
    /// <summary>
    /// Adds a pulsing scale animation to UI elements.
    /// Useful for drawing attention to buttons or interactive elements.
    /// </summary>
    public class PulseEffect : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private float _scaleMultiplier = 1.1f;
        [SerializeField] private bool _playOnEnable = true;
        [SerializeField] private Ease _easeIn = Ease.InOutSine;
        [SerializeField] private Ease _easeOut = Ease.InOutSine;

        [Header("Options")]
        [SerializeField] private bool _loop = true;
        [SerializeField] private int _loopCount = -1; // -1 = infinite

        private Sequence _sequence;
        private Vector3 _originalScale;
        private bool _initialized;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized) return;
            _originalScale = transform.localScale;
            _initialized = true;
        }

        private void OnEnable()
        {
            if (_playOnEnable)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
        }

        /// <summary>
        /// Starts the pulse animation.
        /// </summary>
        public void Play()
        {
            Initialize();
            Stop();

            var targetScale = _originalScale * _scaleMultiplier;
            var halfDuration = _duration * 0.5f;

            _sequence = Sequence.Create(_loop ? _loopCount : 1);
            _sequence.Chain(Tween.Scale(transform, targetScale, halfDuration, _easeIn));
            _sequence.Chain(Tween.Scale(transform, _originalScale, halfDuration, _easeOut));
        }

        /// <summary>
        /// Stops the pulse animation and resets scale.
        /// </summary>
        public void Stop()
        {
            _sequence.Stop();
            if (_initialized)
            {
                transform.localScale = _originalScale;
            }
        }

        /// <summary>
        /// Plays a single pulse animation.
        /// </summary>
        public Sequence PlayOnce()
        {
            Initialize();
            Stop();

            var targetScale = _originalScale * _scaleMultiplier;
            var halfDuration = _duration * 0.5f;

            var seq = Sequence.Create();
            seq.Chain(Tween.Scale(transform, targetScale, halfDuration, _easeIn));
            seq.Chain(Tween.Scale(transform, _originalScale, halfDuration, _easeOut));

            return seq;
        }

        /// <summary>
        /// Sets whether the effect is active.
        /// </summary>
        public void SetActive(bool active)
        {
            if (active)
            {
                Play();
            }
            else
            {
                Stop();
            }
        }

        /// <summary>
        /// Updates the scale multiplier at runtime.
        /// </summary>
        public void SetScaleMultiplier(float multiplier)
        {
            _scaleMultiplier = multiplier;
            if (_sequence.isAlive)
            {
                Play(); // Restart with new scale
            }
        }

        /// <summary>
        /// Updates the duration at runtime.
        /// </summary>
        public void SetDuration(float duration)
        {
            _duration = duration;
            if (_sequence.isAlive)
            {
                Play(); // Restart with new duration
            }
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _duration = 0.5f;
            _scaleMultiplier = 1.1f;
            _playOnEnable = true;
            _loop = true;
            _loopCount = -1;
        }
#endif
    }
}
