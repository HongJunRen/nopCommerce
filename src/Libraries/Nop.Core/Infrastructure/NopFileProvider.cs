using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace Nop.Core.Infrastructure
{
    /// <summary>
    /// IO functions using the on-disk file system
    /// </summary>
    public class NopFileProvider : PhysicalFileProvider, INopFileProvider
    {
        public NopFileProvider(IHostingEnvironment hostingEnvironment) : base(hostingEnvironment.WebRootPath)
        {
            BaseDirectory = hostingEnvironment.ContentRootPath ?? string.Empty;
        }

        protected string BaseDirectory { get; }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
                Directory.CreateDirectory(path);
        }

        public Stream CreateFile(string path)
        {
            return File.Create(path);
        }

        public void DeleteDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(path);

            //find more info about directory deletion
            //and why we use this approach at https://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true
            foreach (var directory in Directory.GetDirectories(path))
                DeleteDirectory(directory);

            try
            {
                Directory.Delete(path, true);
            }
            catch (IOException)
            {
                Directory.Delete(path, true);
            }
            catch (UnauthorizedAccessException)
            {
                Directory.Delete(path, true);
            }
        }

        public void DeleteFile(string filePath)
        {
            if (!FileExists(filePath))
                return;

            File.Delete(filePath);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void DirectoryMove(string sourceDirName, string destDirName)
        {
            Directory.Move(sourceDirName, destDirName);
        }

        public IEnumerable<string> EnumerateFiles(string directoryPath, string searchPattern,
            bool topDirectoryOnly = true)
        {
            return Directory.EnumerateFiles(directoryPath, searchPattern,
                topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        public void FileCopy(string sourceFileName, string destFileName, bool overwrite = false)
        {
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public long FileLength(string path)
        {
            if (!FileExists(path))
                return -1;

            return new FileInfo(path).Length;
        }

        public void FileMove(string sourceFileName, string destFileName)
        {
            File.Move(sourceFileName, destFileName);
        }

        public string GetAbsolutePath(params string[] paths)
        {
            var allPaths = paths.ToList();
            allPaths.Insert(0, Root);

            return Path.Combine(allPaths.ToArray());
        }

        public DateTime GetCreationTime(string path)
        {
            return File.GetCreationTime(path);
        }

        public string[] GetDirectories(string path, string searchPattern = "", bool topDirectoryOnly = true)
        {
            if (string.IsNullOrEmpty(searchPattern))
                searchPattern = "*";

            return Directory.GetDirectories(path, searchPattern,
                topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string GetFileExtension(string filePath)
        {
            return Path.GetExtension(filePath);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetFileNameWithoutExtension(string filePath)
        {
            return Path.GetFileNameWithoutExtension(filePath);
        }

        public string[] GetFiles(string directoryPath, string searchPattern = "", bool topDirectoryOnly = true)
        {
            if (string.IsNullOrEmpty(searchPattern))
                searchPattern = "*.*";

            return Directory.GetFiles(directoryPath, searchPattern,
                topDirectoryOnly ? SearchOption.TopDirectoryOnly : SearchOption.AllDirectories);
        }

        public DateTime GetLastAccessTime(string path)
        {
            return File.GetLastAccessTime(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public DateTime GetLastWriteTimeUtc(string path)
        {
            return File.GetLastWriteTimeUtc(path);
        }

        public string GetParentDirectory(string directoryPath)
        {
            return Directory.GetParent(directoryPath).FullName;
        }

        public bool IsDirectory(string path)
        {
            return DirectoryExists(path);
        }

        public string MapPath(string path)
        {
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(BaseDirectory ?? string.Empty, path);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }

        public byte[] ReadAllBytes(string filePath)
        {
            return File.Exists(filePath) ? File.ReadAllBytes(filePath) : new byte[0];
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            return File.ReadAllText(path, encoding);
        }

        public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
        }

        public void WriteAllBytes(string filePath, byte[] bytes)
        {
            File.WriteAllBytes(filePath, bytes);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            File.WriteAllText(path, contents, encoding);
        }
    }
}