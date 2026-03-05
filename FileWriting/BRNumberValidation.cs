using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace FileWriting
{
    public interface IBRNumberValidation
    {
        bool IsValid(string brNumber);
    }
    public class BRNumberValidation : IBRNumberValidation
    {
        private Regex _brNumberRegex;// = @"\bBR\d{5}\b" OR @“\w[BR]\d{5}”;

        public BRNumberValidation(IOptions<FileWriterSettings> filewriterOptions)
        {
            _brNumberRegex = new Regex(filewriterOptions.Value.BRNumberRegex);
        }
        public bool IsValid(string brNumber) => _brNumberRegex.IsMatch(brNumber);
        
    }
}
