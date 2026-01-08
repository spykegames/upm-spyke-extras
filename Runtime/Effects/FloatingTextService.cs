using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using PrimeTween;

namespace Spyke.Extras.Effects
{
    /// <summary>
    /// Service that manages floating text display with object pooling.
    /// </summary>
    public class FloatingTextService : IFloatingTextService, IInitializable, IDisposable
    {
        [Inject] private readonly DiContainer _container;

        private FloatingText _prefab;
        private Transform _poolParent;
        private Canvas _targetCanvas;
        private Camera _mainCamera;

        private readonly Queue<FloatingText> _pool = new();
        private readonly List<FloatingText> _active = new();
        private const int InitialPoolSize = 5;
        private const int MaxPoolSize = 20;

        public void Initialize()
        {
            _mainCamera = Camera.main;
        }

        /// <summary>
        /// Sets up the service with required components.
        /// Call this after injection with the prefab and canvas.
        /// </summary>
        public void Setup(FloatingText prefab, Canvas targetCanvas, Transform poolParent = null)
        {
            _prefab = prefab;
            _targetCanvas = targetCanvas;
            _poolParent = poolParent ?? targetCanvas.transform;

            // Pre-warm pool
            for (int i = 0; i < InitialPoolSize; i++)
            {
                var instance = CreateInstance();
                ReturnToPool(instance);
            }
        }

        public async UniTask ShowAsync(string text, Vector3 position, FloatingTextConfig config = null)
        {
            config ??= FloatingTextConfig.Default;

            var screenPos = WorldToScreenPosition(position);
            await ShowAtScreenPositionAsync(text, screenPos, config);
        }

        public UniTask ShowAsync(string text, Vector3 position, Color color)
        {
            return ShowAsync(text, position, new FloatingTextConfig { Color = color });
        }

        public async UniTask ShowAtScreenPositionAsync(string text, Vector2 screenPosition, FloatingTextConfig config = null)
        {
            if (_prefab == null || _targetCanvas == null)
            {
                Debug.LogWarning("[FloatingTextService] Service not setup. Call Setup() first.");
                return;
            }

            config ??= FloatingTextConfig.Default;

            var instance = GetFromPool();
            instance.Setup(text, config.Color, config.FontScale);

            // Position
            instance.RectTransform.position = screenPosition;
            instance.gameObject.SetActive(true);
            _active.Add(instance);

            // Animate
            await AnimateFloatingText(instance, config);

            // Return to pool
            _active.Remove(instance);
            ReturnToPool(instance);
        }

        private async UniTask AnimateFloatingText(FloatingText instance, FloatingTextConfig config)
        {
            var sequence = Sequence.Create();

            // Float upward
            var startPos = instance.RectTransform.anchoredPosition;
            var endPos = startPos + Vector2.up * config.FloatDistance;
            sequence.Chain(Tween.UIAnchoredPosition(instance.RectTransform, endPos, config.Duration, Ease.OutQuad));

            // Scale pop (scale up then back down)
            if (config.ScalePop)
            {
                sequence.Group(
                    Tween.Scale(instance.transform, 1.3f, config.Duration * 0.2f, Ease.OutBack)
                        .Chain(Tween.Scale(instance.transform, 1f, config.Duration * 0.3f, Ease.InOutQuad))
                );
            }

            // Fade out
            if (config.FadeOut)
            {
                sequence.Group(
                    Tween.Alpha(instance.CanvasGroup, 0f, config.Duration * 0.4f, Ease.InQuad)
                        .SetDelay(config.Duration * 0.6f)
                );
            }

            await sequence.ToUniTask();
        }

        private Vector2 WorldToScreenPosition(Vector3 worldPosition)
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }

            if (_mainCamera != null)
            {
                return _mainCamera.WorldToScreenPoint(worldPosition);
            }

            return worldPosition;
        }

        private FloatingText GetFromPool()
        {
            if (_pool.Count > 0)
            {
                return _pool.Dequeue();
            }

            return CreateInstance();
        }

        private void ReturnToPool(FloatingText instance)
        {
            instance.Reset();
            instance.gameObject.SetActive(false);

            if (_pool.Count < MaxPoolSize)
            {
                _pool.Enqueue(instance);
            }
            else
            {
                UnityEngine.Object.Destroy(instance.gameObject);
            }
        }

        private FloatingText CreateInstance()
        {
            var instance = _container.InstantiatePrefabForComponent<FloatingText>(_prefab, _poolParent);
            instance.gameObject.SetActive(false);
            return instance;
        }

        public void Dispose()
        {
            // Clear active
            foreach (var active in _active)
            {
                if (active != null)
                {
                    UnityEngine.Object.Destroy(active.gameObject);
                }
            }
            _active.Clear();

            // Clear pool
            while (_pool.Count > 0)
            {
                var pooled = _pool.Dequeue();
                if (pooled != null)
                {
                    UnityEngine.Object.Destroy(pooled.gameObject);
                }
            }
        }
    }
}
