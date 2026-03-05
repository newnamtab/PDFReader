using Microsoft.Extensions.Options;

namespace FileWriting
{
    public class FileWriter : IFileWriter
    {
        private readonly string _fileStorageDirectory;// = "FileStorage";
        private readonly IBRNumberValidation _brNumberValidation;

        public FileWriter(IOptions<FileWriterSettings> fileStorageOption, IBRNumberValidation brNumberValidation)
        {
            _fileStorageDirectory = fileStorageOption.Value.FileStorageDirectory;
            _brNumberValidation = brNumberValidation;
        }

        public async Task<FileWriteResult> SaveFile(string brNumber, HttpContent content)
        {
            
            if ( _brNumberValidation.IsValid(brNumber) == false) { return FileWriteResult.CreateFailure("Invalid BRNumber"); }

            EnsureDirectoryExists();

            // also consider adding error handling for file I/O operations
            // create a new file to write to
            using var file = File.Create($"{_fileStorageDirectory}/{brNumber}.pdf");
            var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream
            await contentStream.CopyToAsync(file); // copy that stream to the file stream

            return FileWriteResult.CreateSuccess();
        }
        private void EnsureDirectoryExists()
        {
            if (!Directory.Exists(_fileStorageDirectory))
            {
                Directory.CreateDirectory(_fileStorageDirectory);
            }
        }
    }
    public class NetworkFileWriter : IFileWriter
    {
        public Task<FileWriteResult> SaveFile(string brNumber, HttpContent content)
        {
            throw new NotImplementedException();
        }
    }
    public class BlobStorageFileWriter : IFileWriter
    {
        public Task<FileWriteResult> SaveFile(string brNumber, HttpContent content)
        {
            throw new NotImplementedException();
        }
    }
}
