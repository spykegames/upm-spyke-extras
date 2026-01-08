using UnityEngine;
using TMPro;

namespace Spyke.Extras.Effects
{
    /// <summary>
    /// MonoBehaviour component for a single floating text instance.
    /// Used with object pooling for performance.
    /// </summary>
    public class FloatingText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _rectTransform;

        private float _defaultFontSize;
        private bool _initialized;

        /// <summary>
        /// The text component.
        /// </summary>
        public TextMeshProUGUI Text => _text;

        /// <summary>
        /// Canvas group for fading.
        /// </summary>
        public CanvasGroup CanvasGroup => _canvasGroup;

        /// <summary>
        /// RectTransform for positioning.
        /// </summary>
        public RectTransform RectTransform => _rectTransform;

        private void Awake()
        {
            if (!_initialized)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            if (_text == null)
            {
                _text = GetComponentInChildren<TextMeshProUGUI>();
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }

            if (_text != null)
            {
                _defaultFontSize = _text.fontSize;
            }

            _initialized = true;
        }

        /// <summary>
        /// Sets up the floating text with the given parameters.
        /// </summary>
        public void Setup(string text, Color color, float fontScale)
        {
            if (!_initialized)
            {
                Initialize();
            }

            if (_text != null)
            {
                _text.text = text;
                _text.color = color;
                _text.fontSize = _defaultFontSize * fontScale;
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }

            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Resets the floating text for pooling.
        /// </summary>
        public void Reset()
        {
            if (_text != null)
            {
                _text.text = string.Empty;
                _text.fontSize = _defaultFontSize;
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
            }

            transform.localScale = Vector3.one;
        }
    }
}
