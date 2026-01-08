using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Spyke.Extras.Icons
{
    /// <summary>
    /// Service interface for icon/sprite management.
    /// Supports sprite atlases and addressables.
    /// </summary>
    public interface IIconService
    {
        /// <summary>
        /// Gets an icon by ID synchronously.
        /// Returns null if not found or not loaded.
        /// </summary>
        /// <param name="iconId">The icon identifier.</param>
        /// <returns>The sprite or null.</returns>
        Sprite GetIcon(string iconId);

        /// <summary>
        /// Gets an icon by ID asynchronously.
        /// Loads from addressables if not already cached.
        /// </summary>
        /// <param name="iconId">The icon identifier.</param>
        /// <returns>The sprite or null if not found.</returns>
        UniTask<Sprite> GetIconAsync(string iconId);

        /// <summary>
        /// Checks if an icon exists.
        /// </summary>
        /// <param name="iconId">The icon identifier.</param>
        /// <returns>True if the icon exists.</returns>
        bool HasIcon(string iconId);

        /// <summary>
        /// Preloads icons for faster access.
        /// </summary>
        /// <param name="iconIds">The icon identifiers to preload.</param>
        UniTask PreloadAsync(params string[] iconIds);

        /// <summary>
        /// Registers an icon with the service.
        /// </summary>
        /// <param name="iconId">The icon identifier.</param>
        /// <param name="sprite">The sprite to register.</param>
        void RegisterIcon(string iconId, Sprite sprite);

        /// <summary>
        /// Unregisters an icon from the service.
        /// </summary>
        /// <param name="iconId">The icon identifier.</param>
        void UnregisterIcon(string iconId);

        /// <summary>
        /// Clears all cached icons.
        /// </summary>
        void ClearCache();
    }
}
