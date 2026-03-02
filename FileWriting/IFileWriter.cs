namespace FileWriting
{
    public interface IFileWriter
    {
        Task SaveFile(string brNumber, HttpContent content);
    }
}
