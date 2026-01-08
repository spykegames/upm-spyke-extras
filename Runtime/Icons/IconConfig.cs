using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Spyke.Extras.Icons
{
    /// <summary>
    /// ScriptableObject configuration for icon management.
    /// Holds references to sprite atlases and individual sprites.
    /// </summary>
    [CreateAssetMenu(fileName = "IconConfig", menuName = "Spyke/Icons/Icon Config")]
    public class IconConfig : ScriptableObject
    {
        [Header("Sprite Atlases")]
        [SerializeField] private List<SpriteAtlas> _spriteAtlases = new();

        [Header("Individual Sprites")]
        [SerializeField] private List<IconEntry> _sprites = new();

        [Header("Settings")]
        [SerializeField] private Sprite _fallbackSprite;

        /// <summary>
        /// The configured sprite atlases.
        /// </summary>
        public IReadOnlyList<SpriteAtlas> SpriteAtlases => _spriteAtlases;

        /// <summary>
        /// The configured individual sprites.
        /// </summary>
        public IReadOnlyList<IconEntry> Sprites => _sprites;

        /// <summary>
        /// Fallback sprite when icon is not found.
        /// </summary>
        public Sprite FallbackSprite => _fallbackSprite;

        /// <summary>
        /// Gets a sprite by ID from atlases or individual sprites.
        /// </summary>
        public Sprite GetSprite(string iconId)
        {
            // Check individual sprites first
            foreach (var entry in _sprites)
            {
                if (entry.Id == iconId)
                {
                    return entry.Sprite;
                }
            }

            // Check sprite atlases
            foreach (var atlas in _spriteAtlases)
            {
                if (atlas == null) continue;
                var sprite = atlas.GetSprite(iconId);
                if (sprite != null)
                {
                    return sprite;
                }
            }

            return _fallbackSprite;
        }

        /// <summary>
        /// Checks if an icon exists.
        /// </summary>
        public bool HasIcon(string iconId)
        {
            // Check individual sprites
            foreach (var entry in _sprites)
            {
                if (entry.Id == iconId)
                {
                    return true;
                }
            }

            // Check atlases
            foreach (var atlas in _spriteAtlases)
            {
                if (atlas == null) continue;
                var sprite = atlas.GetSprite(iconId);
                if (sprite != null)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    /// Entry for an individual icon sprite.
    /// </summary>
    [Serializable]
    public class IconEntry
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _sprite;

        public string Id => _id;
        public Sprite Sprite => _sprite;

        public IconEntry() { }

        public IconEntry(string id, Sprite sprite)
        {
            _id = id;
            _sprite = sprite;
        }
    }
}
