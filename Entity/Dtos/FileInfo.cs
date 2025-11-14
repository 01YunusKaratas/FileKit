using System;

namespace FileKit.Entity.Dtos
{
    public class FileInfoBase
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FileInfo<T> : FileInfoBase
    {
        public T? ExtraData { get; set; }
    }
}