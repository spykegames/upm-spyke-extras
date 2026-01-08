using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace Spyke.Extras.Animation
{
    /// <summary>
    /// Frame-by-frame sprite animator.
    /// Works with both Image and SpriteRenderer.
    /// </summary>
    public class SpriteAnimator : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Image _targetImage;
        [SerializeField] private SpriteRenderer _targetSpriteRenderer;

        [Header("Animation")]
        [SerializeField] private List<Sprite> _frames = new();
        [SerializeField] private float _frameRate = 12f;
        [SerializeField] private bool _loop = true;
        [SerializeField] private bool _playOnEnable = true;

        private int _currentFrame;
        private float _timer;
        private bool _isPlaying;
        private bool _isPaused;

        /// <summary>
        /// Current frame index.
        /// </summary>
        public int CurrentFrame => _currentFrame;

        /// <summary>
        /// Whether animation is playing.
        /// </summary>
        public bool IsPlaying => _isPlaying;

        /// <summary>
        /// Total number of frames.
        /// </summary>
        public int FrameCount => _frames.Count;

        /// <summary>
        /// Event fired when animation completes (non-looping).
        /// </summary>
        public event Action OnComplete;

        /// <summary>
        /// Event fired on each frame change.
        /// </summary>
        public event Action<int> OnFrameChanged;

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

        private void Update()
        {
            if (!_isPlaying || _isPaused || _frames.Count == 0) return;

            _timer += Time.deltaTime;
            var frameDuration = 1f / _frameRate;

            if (_timer >= frameDuration)
            {
                _timer -= frameDuration;
                AdvanceFrame();
            }
        }

        private void AdvanceFrame()
        {
            _currentFrame++;

            if (_currentFrame >= _frames.Count)
            {
                if (_loop)
                {
                    _currentFrame = 0;
                }
                else
                {
                    _currentFrame = _frames.Count - 1;
                    _isPlaying = false;
                    OnComplete?.Invoke();
                    return;
                }
            }

            ApplyFrame(_currentFrame);
            OnFrameChanged?.Invoke(_currentFrame);
        }

        private void ApplyFrame(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex >= _frames.Count) return;

            var sprite = _frames[frameIndex];

            if (_targetImage != null)
            {
                _targetImage.sprite = sprite;
            }

            if (_targetSpriteRenderer != null)
            {
                _targetSpriteRenderer.sprite = sprite;
            }
        }

        /// <summary>
        /// Plays the animation from the start.
        /// </summary>
        public void Play()
        {
            _currentFrame = 0;
            _timer = 0f;
            _isPlaying = true;
            _isPaused = false;
            ApplyFrame(0);
        }

        /// <summary>
        /// Pauses the animation.
        /// </summary>
        public void Pause()
        {
            _isPaused = true;
        }

        /// <summary>
        /// Resumes a paused animation.
        /// </summary>
        public void Resume()
        {
            _isPaused = false;
        }

        /// <summary>
        /// Stops the animation.
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
            _isPaused = false;
            _currentFrame = 0;
            _timer = 0f;
        }

        /// <summary>
        /// Sets the frame rate.
        /// </summary>
        public void SetFrameRate(float fps)
        {
            _frameRate = Mathf.Max(0.1f, fps);
        }

        /// <summary>
        /// Sets the frames.
        /// </summary>
        public void SetFrames(IList<Sprite> frames)
        {
            _frames.Clear();
            _frames.AddRange(frames);
        }

        /// <summary>
        /// Sets whether to loop.
        /// </summary>
        public void SetLoop(bool loop)
        {
            _loop = loop;
        }

        /// <summary>
        /// Goes to a specific frame.
        /// </summary>
        public void GoToFrame(int frameIndex)
        {
            if (frameIndex < 0 || frameIndex >= _frames.Count) return;

            _currentFrame = frameIndex;
            ApplyFrame(frameIndex);
        }

        /// <summary>
        /// Plays animation and waits for completion.
        /// </summary>
        public async UniTask PlayAsync()
        {
            if (_loop)
            {
                Debug.LogWarning("[SpriteAnimator] PlayAsync called on looping animation. Set loop to false.");
                return;
            }

            Play();
            await UniTask.WaitUntil(() => !_isPlaying);
        }

#if UNITY_EDITOR
        private void Reset()
        {
            _targetImage = GetComponent<Image>();
            _targetSpriteRenderer = GetComponent<SpriteRenderer>();
        }
#endif
    }
}
