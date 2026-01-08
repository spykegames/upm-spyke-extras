using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using Zenject;

#if ADDRESSABLES_ENABLED
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Spyke.Extras.Icons
{
    /// <summary>
    /// Service implementation for icon/sprite management.
    /// Supports sprite atlases and addressables.
    /// </summary>
    public class IconService : IIconService, IInitializable, IDisposable
    {
        [Inject(Optional = true)] private readonly IconConfig _config;

        private readonly Dictionary<string, Sprite> _cache = new();
        private readonly Dictionary<string, SpriteAtlas> _loadedAtlases = new();
        private bool _initialized;

        public void Initialize()
        {
            if (_config != null)
            {
                // Pre-register sprites from config
                foreach (var entry in _config.Sprites)
                {
                    if (!string.IsNullOrEmpty(entry.Id) && entry.Sprite != null)
                    {
                        _cache[entry.Id] = entry.Sprite;
                    }
                }
            }

            // Register for atlas requests
            SpriteAtlasManager.atlasRequested += OnAtlasRequested;
            SpriteAtlasManager.atlasRegistered += OnAtlasRegistered;

            _initialized = true;
        }

        public Sprite GetIcon(string iconId)
        {
            if (string.IsNullOrEmpty(iconId)) return null;

            // Check cache
            if (_cache.TryGetValue(iconId, out var cached))
            {
                return cached;
            }

            // Check config
            if (_config != null)
            {
                var sprite = _config.GetSprite(iconId);
                if (sprite != null)
                {
                    _cache[iconId] = sprite;
                    return sprite;
                }
            }

            return _config?.FallbackSprite;
        }

        public async UniTask<Sprite> GetIconAsync(string iconId)
        {
            if (string.IsNullOrEmpty(iconId)) return null;

            // Check cache first
            if (_cache.TryGetValue(iconId, out var cached))
            {
                return cached;
            }

            // Check config
            if (_config != null)
            {
                var sprite = _config.GetSprite(iconId);
                if (sprite != null)
                {
                    _cache[iconId] = sprite;
                    return sprite;
                }
            }

#if ADDRESSABLES_ENABLED
            // Try loading from addressables
            try
            {
                var handle = Addressables.LoadAssetAsync<Sprite>(iconId);
                var sprite = await handle.ToUniTask();
                if (sprite != null)
                {
                    _cache[iconId] = sprite;
                    return sprite;
                }
            }
            catch (Exception)
            {
                // Addressable not found, fall through
            }
#endif

            return _config?.FallbackSprite;
        }

        public bool HasIcon(string iconId)
        {
            if (string.IsNullOrEmpty(iconId)) return false;

            // Check cache
            if (_cache.ContainsKey(iconId))
            {
                return true;
            }

            // Check config
            if (_config != null && _config.HasIcon(iconId))
            {
                return true;
            }

            return false;
        }

        public async UniTask PreloadAsync(params string[] iconIds)
        {
            if (iconIds == null || iconIds.Length == 0) return;

            var tasks = new List<UniTask>();
            foreach (var iconId in iconIds)
            {
                if (!_cache.ContainsKey(iconId))
                {
                    tasks.Add(GetIconAsync(iconId).AsUniTask());
                }
            }

            if (tasks.Count > 0)
            {
                await UniTask.WhenAll(tasks);
            }
        }

        public void RegisterIcon(string iconId, Sprite sprite)
        {
            if (string.IsNullOrEmpty(iconId) || sprite == null) return;
            _cache[iconId] = sprite;
        }

        public void UnregisterIcon(string iconId)
        {
            if (string.IsNullOrEmpty(iconId)) return;
            _cache.Remove(iconId);
        }

        public void ClearCache()
        {
            _cache.Clear();

            // Re-register from config
            if (_config != null)
            {
                foreach (var entry in _config.Sprites)
                {
                    if (!string.IsNullOrEmpty(entry.Id) && entry.Sprite != null)
                    {
                        _cache[entry.Id] = entry.Sprite;
                    }
                }
            }
        }

        private void OnAtlasRequested(string atlasTag, Action<SpriteAtlas> callback)
        {
            // Try to load atlas from config
            if (_config != null)
            {
                foreach (var atlas in _config.SpriteAtlases)
                {
                    if (atlas != null && atlas.tag == atlasTag)
                    {
                        callback?.Invoke(atlas);
                        return;
                    }
                }
            }

#if ADDRESSABLES_ENABLED
            // Try loading from addressables
            LoadAtlasFromAddressables(atlasTag, callback).Forget();
#endif
        }

        private void OnAtlasRegistered(SpriteAtlas atlas)
        {
            if (atlas != null && !string.IsNullOrEmpty(atlas.tag))
            {
                _loadedAtlases[atlas.tag] = atlas;
            }
        }

#if ADDRESSABLES_ENABLED
        private async UniTaskVoid LoadAtlasFromAddressables(string atlasTag, Action<SpriteAtlas> callback)
        {
            try
            {
                var handle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasTag);
                var atlas = await handle.ToUniTask();
                if (atlas != null)
                {
                    _loadedAtlases[atlasTag] = atlas;
                    callback?.Invoke(atlas);
                }
            }
            catch (Exception)
            {
                // Atlas not found in addressables
            }
        }
#endif

        public void Dispose()
        {
            SpriteAtlasManager.atlasRequested -= OnAtlasRequested;
            SpriteAtlasManager.atlasRegistered -= OnAtlasRegistered;

            _cache.Clear();
            _loadedAtlases.Clear();
        }
    }
}
