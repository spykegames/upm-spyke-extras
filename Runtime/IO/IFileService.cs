using Cysharp.Threading.Tasks;

namespace Spyke.Extras.IO
{
    /// <summary>
    /// Service interface for file I/O operations.
    /// Provides cross-platform file access.
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// Reads text from a file asynchronously.
        /// </summary>
        /// <param name="path">The file path (can be relative to persistent data).</param>
        /// <returns>The file contents or null if not found.</returns>
        UniTask<string> ReadTextAsync(string path);

        /// <summary>
        /// Writes text to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="content">The text content.</param>
        UniTask WriteTextAsync(string path, string content);

        /// <summary>
        /// Reads bytes from a file asynchronously.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>The file bytes or null if not found.</returns>
        UniTask<byte[]> ReadBytesAsync(string path);

        /// <summary>
        /// Writes bytes to a file asynchronously.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <param name="data">The byte data.</param>
        UniTask WriteBytesAsync(string path, byte[] data);

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>True if the file exists.</returns>
        bool Exists(string path);

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="path">The file path.</param>
        /// <returns>True if deleted successfully.</returns>
        bool Delete(string path);

        /// <summary>
        /// Ensures a directory exists.
        /// </summary>
        /// <param name="path">The directory path.</param>
        void EnsureDirectory(string path);

        /// <summary>
        /// Gets the full path for a relative path.
        /// </summary>
        /// <param name="relativePath">The relative path.</param>
        /// <returns>The full path in persistent data path.</returns>
        string GetFullPath(string relativePath);
    }
}
