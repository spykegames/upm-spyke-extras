using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Spyke.Extras.ImageRepo
{
    /// <summary>
    /// Service interface for loading and caching remote images.
    /// </summary>
    public interface IImageRepository
    {
        /// <summary>
        /// Loads an image from URL asynchronously.
        /// Uses memory and disk cache for performance.
        /// </summary>
        /// <param name="url">The image URL.</param>
        /// <param name="forceDownload">If true, bypasses cache and re-downloads.</param>
        /// <returns>The loaded texture or null if failed.</returns>
        UniTask<Texture2D> LoadAsync(string url, bool forceDownload = false);

        /// <summary>
        /// Loads an image and applies it to a RawImage component.
        /// </summary>
        /// <param name="url">The image URL.</param>
        /// <param name="target">The target RawImage.</param>
        /// <param name="placeholder">Optional placeholder texture while loading.</param>
        /// <param name="forceDownload">If true, bypasses cache.</param>
        void LoadInto(string url, RawImage target, Texture2D placeholder = null, bool forceDownload = false);

        /// <summary>
        /// Loads an image and applies it to an Image component (as sprite).
        /// </summary>
        /// <param name="url">The image URL.</param>
        /// <param name="target">The target Image.</param>
        /// <param name="placeholder">Optional placeholder sprite while loading.</param>
        /// <param name="forceDownload">If true, bypasses cache.</param>
        void LoadInto(string url, Image target, Sprite placeholder = null, bool forceDownload = false);

        /// <summary>
        /// Checks if an image is cached.
        /// </summary>
        /// <param name="url">The image URL.</param>
        /// <returns>True if cached in memory or disk.</returns>
        bool IsCached(string url);

        /// <summary>
        /// Gets an image from cache only (does not download).
        /// </summary>
        /// <param name="url">The image URL.</param>
        /// <returns>The cached texture or null.</returns>
        UniTask<Texture2D> GetFromCacheAsync(string url);

        /// <summary>
        /// Saves a texture to cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="texture">The texture to cache.</param>
        void SaveToCache(string key, Texture2D texture);

        /// <summary>
        /// Clears all cached images.
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Clears a specific cached image.
        /// </summary>
        /// <param name="url">The image URL to clear.</param>
        void ClearCache(string url);
    }
}
