using System;
using System.IO;
using Microsoft.Extensions.FileProviders;

namespace Nop.Core.Infrastructure
{
    public class NopFileInfo : IFileInfo
    {
        private readonly INopFileProvider _fileProvider;

        public NopFileInfo(INopFileProvider fileProvider, string path)
        {
            this._fileProvider = fileProvider;
            PhysicalPath = path;
        }

        public Stream CreateReadStream()
        {
            return _fileProvider.OpenRead(PhysicalPath);
        }

        public bool Exists { get { return _fileProvider.FileExists(PhysicalPath); } }
        public long Length { get { return _fileProvider.FileLength(PhysicalPath); } }
        public string PhysicalPath { get; }
        public string Name { get { return _fileProvider.GetFileName(PhysicalPath); } }
        public DateTimeOffset LastModified { get { return _fileProvider.GetLastWriteTimeUtc(PhysicalPath); } }
        public bool IsDirectory { get { return _fileProvider.IsDirectory(PhysicalPath); } }
    }
}