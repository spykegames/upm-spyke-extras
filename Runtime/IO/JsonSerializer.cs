using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.Extras.IO
{
    /// <summary>
    /// JSON serialization helper using Unity's JsonUtility.
    /// For more complex scenarios, use Newtonsoft.Json.
    /// </summary>
    public class JsonSerializer
    {
        [Inject(Optional = true)] private readonly IFileService _fileService;

        /// <summary>
        /// Serializes an object to JSON string.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="prettyPrint">Whether to format the output.</param>
        /// <returns>The JSON string.</returns>
        public string Serialize<T>(T obj, bool prettyPrint = false)
        {
            try
            {
                return JsonUtility.ToJson(obj, prettyPrint);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[JsonSerializer] Failed to serialize: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deserializes a JSON string to an object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>The deserialized object or default.</returns>
        public T Deserialize<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            try
            {
                return JsonUtility.FromJson<T>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[JsonSerializer] Failed to deserialize: {e.Message}");
                return default;
            }
        }

        /// <summary>
        /// Deserializes JSON and overwrites an existing object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <param name="target">The object to overwrite.</param>
        public void DeserializeOverwrite<T>(string json, T target) where T : class
        {
            if (string.IsNullOrEmpty(json) || target == null)
            {
                return;
            }

            try
            {
                JsonUtility.FromJsonOverwrite(json, target);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[JsonSerializer] Failed to deserialize overwrite: {e.Message}");
            }
        }

        /// <summary>
        /// Saves an object to a JSON file.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="path">The file path.</param>
        /// <param name="obj">The object to save.</param>
        /// <param name="prettyPrint">Whether to format the output.</param>
        public async UniTask SaveAsync<T>(string path, T obj, bool prettyPrint = false)
        {
            if (_fileService == null)
            {
                Debug.LogWarning("[JsonSerializer] FileService not available.");
                return;
            }

            var json = Serialize(obj, prettyPrint);
            if (!string.IsNullOrEmpty(json))
            {
                await _fileService.WriteTextAsync(path, json);
            }
        }

        /// <summary>
        /// Loads an object from a JSON file.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="path">The file path.</param>
        /// <returns>The deserialized object or default.</returns>
        public async UniTask<T> LoadAsync<T>(string path)
        {
            if (_fileService == null)
            {
                Debug.LogWarning("[JsonSerializer] FileService not available.");
                return default;
            }

            var json = await _fileService.ReadTextAsync(path);
            return Deserialize<T>(json);
        }

        /// <summary>
        /// Checks if a JSON file exists.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>True if the file exists.</returns>
        public bool Exists(string path)
        {
            return _fileService?.Exists(path) ?? false;
        }

        /// <summary>
        /// Deletes a JSON file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>True if deleted.</returns>
        public bool Delete(string path)
        {
            return _fileService?.Delete(path) ?? false;
        }
    }
}
