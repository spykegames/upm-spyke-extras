using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

namespace Spyke.Extras.Effects
{
    /// <summary>
    /// Adds a shiny sweep effect to UI elements.
    /// Uses a gradient mask that animates across the element.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class ShinyEffect : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private float _duration = 1f;
        [SerializeField] private float _delay = 0.5f;
        [SerializeField] private float _interval = 2.5f;
        [SerializeField] private bool _playOnEnable = true;

        [Header("Shine Settings")]
        [SerializeField] private Material _shinyMaterial;
        [SerializeField, Range(0f, 1f)] private float _shineWidth = 0.2f;
        [SerializeField] private Color _shineColor = new Color(1f, 1f, 1f, 0.5f);

        private Image _image;
        private Material _materialInstance;
        private Sequence _sequence;
        private float _location;

        private static readonly int LocationProperty = Shader.PropertyToID("_Location");
        private static readonly int WidthProperty = Shader.PropertyToID("_Width");
        private static readonly int ColorProperty = Shader.PropertyToID("_ShineColor");

        private void Awake()
        {
            _image = GetComponent<Image>();

            if (_shinyMaterial != null)
            {
                _materialInstance = new Material(_shinyMaterial);
                _image.material = _materialInstance;
                _materialInstance.SetFloat(WidthProperty, _shineWidth);
                _materialInstance.SetColor(ColorProperty, _shineColor);
            }
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

        private void OnDestroy()
        {
            if (_materialInstance != null)
            {
                Destroy(_materialInstance);
            }
        }

        /// <summary>
        /// Starts the shiny animation loop.
        /// </summary>
        public void Play()
        {
            Stop();

            if (_materialInstance == null) return;

            _location = 0f;
            UpdateMaterial();

            _sequence = Sequence.Create(cycles: -1);
            _sequence.ChainDelay(_delay);
            _sequence.Chain(
                Tween.Custom(0f, 1f, _duration, onValueChange: value =>
                {
                    _location = value;
                    UpdateMaterial();
                }, ease: Ease.Linear)
            );
            _sequence.ChainDelay(_interval);
        }

        /// <summary>
        /// Stops the shiny animation.
        /// </summary>
        public void Stop()
        {
            _sequence.Stop();
            _location = 0f;
            UpdateMaterial();
        }

        /// <summary>
        /// Plays a single shine animation.
        /// </summary>
        public Tween PlayOnce()
        {
            Stop();

            if (_materialInstance == null) return default;

            _location = 0f;
            UpdateMaterial();

            return Tween.Custom(0f, 1f, _duration, onValueChange: value =>
            {
                _location = value;
                UpdateMaterial();
            }, ease: Ease.Linear);
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

        private void UpdateMaterial()
        {
            if (_materialInstance != null)
            {
                _materialInstance.SetFloat(LocationProperty, _location);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_materialInstance != null)
            {
                _materialInstance.SetFloat(WidthProperty, _shineWidth);
                _materialInstance.SetColor(ColorProperty, _shineColor);
            }
        }
#endif
    }
}
