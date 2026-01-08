using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Spyke.Extras.IO
{
    /// <summary>
    /// Implementation of IFileService with cross-platform support.
    /// </summary>
    public class FileService : IFileService, IInitializable
    {
        private string _basePath;

        public void Initialize()
        {
            _basePath = Application.persistentDataPath;
        }

        public async UniTask<string> ReadTextAsync(string path)
        {
            var fullPath = GetFullPath(path);

            if (!File.Exists(fullPath))
            {
                return null;
            }

            try
            {
                return await File.ReadAllTextAsync(fullPath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FileService] Failed to read text: {path} - {e.Message}");
                return null;
            }
        }

        public async UniTask WriteTextAsync(string path, string content)
        {
            var fullPath = GetFullPath(path);

            try
            {
                EnsureDirectory(Path.GetDirectoryName(fullPath));
                await File.WriteAllTextAsync(fullPath, content ?? string.Empty);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FileService] Failed to write text: {path} - {e.Message}");
            }
        }

        public async UniTask<byte[]> ReadBytesAsync(string path)
        {
            var fullPath = GetFullPath(path);

            if (!File.Exists(fullPath))
            {
                return null;
            }

            try
            {
                return await File.ReadAllBytesAsync(fullPath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FileService] Failed to read bytes: {path} - {e.Message}");
                return null;
            }
        }

        public async UniTask WriteBytesAsync(string path, byte[] data)
        {
            var fullPath = GetFullPath(path);

            try
            {
                EnsureDirectory(Path.GetDirectoryName(fullPath));
                await File.WriteAllBytesAsync(fullPath, data ?? Array.Empty<byte>());
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FileService] Failed to write bytes: {path} - {e.Message}");
            }
        }

        public bool Exists(string path)
        {
            var fullPath = GetFullPath(path);
            return File.Exists(fullPath);
        }

        public bool Delete(string path)
        {
            var fullPath = GetFullPath(path);

            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FileService] Failed to delete: {path} - {e.Message}");
            }

            return false;
        }

        public void EnsureDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return;

            var fullPath = Path.IsPathRooted(path) ? path : GetFullPath(path);

            try
            {
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[FileService] Failed to create directory: {path} - {e.Message}");
            }
        }

        public string GetFullPath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return _basePath;
            }

            // If already absolute, return as-is
            if (Path.IsPathRooted(relativePath))
            {
                return relativePath;
            }

            return Path.Combine(_basePath, relativePath);
        }
    }
}
