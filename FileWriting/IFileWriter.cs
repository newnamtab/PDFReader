namespace FileWriting
{
    public interface IFileWriter
    {
        Task<FileWriteResult> SaveFile(string brNumber, HttpContent content);
    }
    public struct FileWriteResult
    {
        public bool Success { get; }
        public string ErrorMessage { get; }
        private FileWriteResult(bool success, string errorMessage)
        {
            Success = success;
            ErrorMessage = errorMessage;
        }
        public static FileWriteResult CreateSuccess() => new FileWriteResult(true, string.Empty);
        public static FileWriteResult CreateFailure(string errorMessage) => new FileWriteResult(false, errorMessage);
    }
}
