namespace PdfUrlExaminer
{
    public interface IFileWriter
    {
        Task SaveFile(string brNumber, HttpContent content);
    }
    public class FileWriter : IFileWriter
    {
        public async Task SaveFile(string brNumber, HttpContent content)
        {
            // create a new file to write to
            // TODO: consider using a more robust file naming strategy to avoid potential conflicts or invalid characters in brNumber
            // also consider adding error handling for file I/O operations
            // ensure the directory exists
            // TODO: Make the base directory configurable rather than hardcoding "FileStorage"
            using var file = File.Create($"FileStorage/{brNumber}.pdf");
            var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream
            await contentStream.CopyToAsync(file); // copy that stream to the file stream

            //TODO: Return some indication of success or failure
        }
    }
}
