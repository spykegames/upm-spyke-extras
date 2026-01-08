using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Spyke.Extras.Effects
{
    /// <summary>
    /// Service interface for displaying floating text (damage numbers, rewards, etc.).
    /// </summary>
    public interface IFloatingTextService
    {
        /// <summary>
        /// Shows floating text at the specified position.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="position">World position.</param>
        /// <param name="config">Optional configuration.</param>
        /// <returns>Task that completes when the animation finishes.</returns>
        UniTask ShowAsync(string text, Vector3 position, FloatingTextConfig config = null);

        /// <summary>
        /// Shows floating text with a specific color.
        /// </summary>
        UniTask ShowAsync(string text, Vector3 position, Color color);

        /// <summary>
        /// Shows floating text at a screen position.
        /// </summary>
        UniTask ShowAtScreenPositionAsync(string text, Vector2 screenPosition, FloatingTextConfig config = null);
    }

    /// <summary>
    /// Configuration for floating text appearance and animation.
    /// </summary>
    public class FloatingTextConfig
    {
        /// <summary>
        /// Text color.
        /// </summary>
        public Color Color { get; set; } = Color.white;

        /// <summary>
        /// Font size multiplier (1.0 = default).
        /// </summary>
        public float FontScale { get; set; } = 1f;

        /// <summary>
        /// Duration of the float animation in seconds.
        /// </summary>
        public float Duration { get; set; } = 1f;

        /// <summary>
        /// Vertical distance to float.
        /// </summary>
        public float FloatDistance { get; set; } = 100f;

        /// <summary>
        /// Whether to fade out during animation.
        /// </summary>
        public bool FadeOut { get; set; } = true;

        /// <summary>
        /// Whether to scale up then down during animation.
        /// </summary>
        public bool ScalePop { get; set; } = true;

        /// <summary>
        /// Default configuration.
        /// </summary>
        public static FloatingTextConfig Default => new FloatingTextConfig();

        /// <summary>
        /// Configuration for damage numbers.
        /// </summary>
        public static FloatingTextConfig Damage => new FloatingTextConfig
        {
            Color = Color.red,
            FontScale = 1.2f,
            ScalePop = true
        };

        /// <summary>
        /// Configuration for reward/heal numbers.
        /// </summary>
        public static FloatingTextConfig Reward => new FloatingTextConfig
        {
            Color = Color.yellow,
            FontScale = 1.1f,
            ScalePop = true
        };
    }
}
