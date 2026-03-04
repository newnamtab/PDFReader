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

        public BRNumberValidation(string regex)
        {
            _brNumberRegex = new Regex(regex);
        }
        public bool IsValid(string brNumber) => _brNumberRegex.IsMatch(brNumber);
        
    }
}
