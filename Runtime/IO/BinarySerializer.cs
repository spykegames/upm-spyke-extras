using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.Extras.IO
{
    /// <summary>
    /// Binary serialization helper.
    /// Objects must be marked with [Serializable] attribute.
    /// </summary>
    public class BinarySerializer
    {
        [Inject(Optional = true)] private readonly IFileService _fileService;

        /// <summary>
        /// Serializes an object to bytes.
        /// </summary>
        /// <typeparam name="T">The object type (must be [Serializable]).</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized bytes or null.</returns>
        public byte[] Serialize<T>(T obj)
        {
            if (obj == null) return null;

            try
            {
                using var stream = new MemoryStream();
#pragma warning disable SYSLIB0011
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
#pragma warning restore SYSLIB0011
                return stream.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[BinarySerializer] Failed to serialize: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deserializes bytes to an object.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="data">The serialized bytes.</param>
        /// <returns>The deserialized object or default.</returns>
        public T Deserialize<T>(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                return default;
            }

            try
            {
                using var stream = new MemoryStream(data);
#pragma warning disable SYSLIB0011
                var formatter = new BinaryFormatter();
                var result = formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
                if (result is T typedResult)
                {
                    return typedResult;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[BinarySerializer] Failed to deserialize: {e.Message}");
            }

            return default;
        }

        /// <summary>
        /// Saves an object to a binary file.
        /// </summary>
        /// <typeparam name="T">The object type.</typeparam>
        /// <param name="path">The file path.</param>
        /// <param name="obj">The object to save.</param>
        public async UniTask SaveAsync<T>(string path, T obj)
        {
            if (_fileService == null)
            {
                Debug.LogWarning("[BinarySerializer] FileService not available.");
                return;
            }

            var bytes = Serialize(obj);
            if (bytes != null)
            {
                await _fileService.WriteBytesAsync(path, bytes);
            }
        }

        /// <summary>
        /// Loads an object from a binary file.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="path">The file path.</param>
        /// <returns>The deserialized object or default.</returns>
        public async UniTask<T> LoadAsync<T>(string path)
        {
            if (_fileService == null)
            {
                Debug.LogWarning("[BinarySerializer] FileService not available.");
                return default;
            }

            var bytes = await _fileService.ReadBytesAsync(path);
            return Deserialize<T>(bytes);
        }

        /// <summary>
        /// Checks if a binary file exists.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>True if the file exists.</returns>
        public bool Exists(string path)
        {
            return _fileService?.Exists(path) ?? false;
        }

        /// <summary>
        /// Deletes a binary file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>True if deleted.</returns>
        public bool Delete(string path)
        {
            return _fileService?.Delete(path) ?? false;
        }
    }
}
