using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Spyke.Services.Cache;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Zenject;

namespace Spyke.Extras.ImageRepo
{
    /// <summary>
    /// Implementation of IImageRepository with memory and disk caching.
    /// </summary>
    public class ImageRepository : IImageRepository, IInitializable, IDisposable
    {
        [Inject(Optional = true)] private readonly IDiskCache _diskCache;

        private readonly Dictionary<string, Texture2D> _memoryCache = new();
        private readonly Dictionary<string, UniTask<Texture2D>> _pendingRequests = new();
        private readonly object _lock = new();

        private const float DefaultTimeout = 10f;
        private const int MaxMemoryCacheSize = 50;

        private bool _initialized;

        public void Initialize()
        {
            _diskCache?.Initialize();
            _initialized = true;
        }

        public async UniTask<Texture2D> LoadAsync(string url, bool forceDownload = false)
        {
            if (string.IsNullOrEmpty(url)) return null;

            // Check memory cache
            if (!forceDownload)
            {
                lock (_lock)
                {
                    if (_memoryCache.TryGetValue(url, out var cached))
                    {
                        return cached;
                    }
                }
            }

            // Check if request is already pending
            lock (_lock)
            {
                if (_pendingRequests.TryGetValue(url, out var pending))
                {
                    return await pending;
                }
            }

            // Start new request
            var task = LoadAsyncInternal(url, forceDownload);
            lock (_lock)
            {
                _pendingRequests[url] = task;
            }

            try
            {
                return await task;
            }
            finally
            {
                lock (_lock)
                {
                    _pendingRequests.Remove(url);
                }
            }
        }

        private async UniTask<Texture2D> LoadAsyncInternal(string url, bool forceDownload)
        {
            // Check disk cache
            if (!forceDownload && _diskCache != null)
            {
                var cachedBytes = await _diskCache.GetAsync(url);
                if (cachedBytes != null && cachedBytes.Length > 0)
                {
                    var texture = CreateTextureFromBytes(cachedBytes);
                    if (texture != null)
                    {
                        CacheInMemory(url, texture);
                        return texture;
                    }
                }
            }

            // Download from URL
            return await DownloadImageAsync(url);
        }

        private async UniTask<Texture2D> DownloadImageAsync(string url)
        {
            using var request = UnityWebRequestTexture.GetTexture(url);
            request.timeout = (int)DefaultTimeout;

            try
            {
                await request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[ImageRepository] Failed to load image: {url} - {request.error}");
                    return null;
                }

                var texture = DownloadHandlerTexture.GetContent(request);
                if (texture != null)
                {
                    CacheInMemory(url, texture);
                    await CacheToDiskAsync(url, texture);
                }

                return texture;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ImageRepository] Exception loading image: {url} - {e.Message}");
                return null;
            }
        }

        public void LoadInto(string url, RawImage target, Texture2D placeholder = null, bool forceDownload = false)
        {
            if (target == null) return;

            if (placeholder != null)
            {
                target.texture = placeholder;
            }

            LoadIntoRawImageAsync(url, target, forceDownload).Forget();
        }

        private async UniTaskVoid LoadIntoRawImageAsync(string url, RawImage target, bool forceDownload)
        {
            var texture = await LoadAsync(url, forceDownload);

            if (target != null && target.gameObject != null)
            {
                target.texture = texture;
            }
        }

        public void LoadInto(string url, Image target, Sprite placeholder = null, bool forceDownload = false)
        {
            if (target == null) return;

            if (placeholder != null)
            {
                target.sprite = placeholder;
            }

            LoadIntoImageAsync(url, target, forceDownload).Forget();
        }

        private async UniTaskVoid LoadIntoImageAsync(string url, Image target, bool forceDownload)
        {
            var texture = await LoadAsync(url, forceDownload);

            if (target != null && target.gameObject != null && texture != null)
            {
                var sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                target.sprite = sprite;
            }
        }

        public bool IsCached(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            lock (_lock)
            {
                if (_memoryCache.ContainsKey(url))
                {
                    return true;
                }
            }

            if (_diskCache != null)
            {
                var bytes = _diskCache.Get(url);
                return bytes != null && bytes.Length > 0;
            }

            return false;
        }

        public async UniTask<Texture2D> GetFromCacheAsync(string url)
        {
            if (string.IsNullOrEmpty(url)) return null;

            // Check memory cache
            lock (_lock)
            {
                if (_memoryCache.TryGetValue(url, out var cached))
                {
                    return cached;
                }
            }

            // Check disk cache
            if (_diskCache != null)
            {
                var bytes = await _diskCache.GetAsync(url);
                if (bytes != null && bytes.Length > 0)
                {
                    var texture = CreateTextureFromBytes(bytes);
                    if (texture != null)
                    {
                        CacheInMemory(url, texture);
                        return texture;
                    }
                }
            }

            return null;
        }

        public void SaveToCache(string key, Texture2D texture)
        {
            if (string.IsNullOrEmpty(key) || texture == null) return;

            CacheInMemory(key, texture);

            if (texture.isReadable)
            {
                CacheToDiskAsync(key, texture).Forget();
            }
        }

        public void ClearCache()
        {
            lock (_lock)
            {
                foreach (var texture in _memoryCache.Values)
                {
                    if (texture != null)
                    {
                        UnityEngine.Object.Destroy(texture);
                    }
                }
                _memoryCache.Clear();
            }

            _diskCache?.Clear();
        }

        public void ClearCache(string url)
        {
            if (string.IsNullOrEmpty(url)) return;

            lock (_lock)
            {
                if (_memoryCache.TryGetValue(url, out var texture))
                {
                    if (texture != null)
                    {
                        UnityEngine.Object.Destroy(texture);
                    }
                    _memoryCache.Remove(url);
                }
            }

            _diskCache?.Remove(url);
        }

        private void CacheInMemory(string url, Texture2D texture)
        {
            lock (_lock)
            {
                // Evict oldest if at capacity
                if (_memoryCache.Count >= MaxMemoryCacheSize)
                {
                    // Simple eviction: remove first item
                    using var enumerator = _memoryCache.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        var toRemove = enumerator.Current.Key;
                        if (_memoryCache.TryGetValue(toRemove, out var oldTexture))
                        {
                            if (oldTexture != null)
                            {
                                UnityEngine.Object.Destroy(oldTexture);
                            }
                        }
                        _memoryCache.Remove(toRemove);
                    }
                }

                _memoryCache[url] = texture;
            }
        }

        private async UniTask CacheToDiskAsync(string url, Texture2D texture)
        {
            if (_diskCache == null || !texture.isReadable) return;

            try
            {
                var bytes = texture.EncodeToPNG();
                if (bytes != null && bytes.Length > 0)
                {
                    await _diskCache.PutAsync(url, bytes);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ImageRepository] Failed to cache to disk: {e.Message}");
            }
        }

        private Texture2D CreateTextureFromBytes(byte[] bytes)
        {
            try
            {
                var texture = new Texture2D(2, 2);
                if (texture.LoadImage(bytes))
                {
                    return texture;
                }
                UnityEngine.Object.Destroy(texture);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ImageRepository] Failed to create texture: {e.Message}");
            }
            return null;
        }

        public void Dispose()
        {
            lock (_lock)
            {
                foreach (var texture in _memoryCache.Values)
                {
                    if (texture != null)
                    {
                        UnityEngine.Object.Destroy(texture);
                    }
                }
                _memoryCache.Clear();
                _pendingRequests.Clear();
            }
        }
    }
}
