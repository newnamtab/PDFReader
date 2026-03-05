using ExcelDataReader;
using Microsoft.Extensions.Options;

namespace URLProvider
{
    public interface IURLProvider
    {
        IEnumerable<ProvidedUrls> GetURLs();
    }
    public class URLProvider: IURLProvider
    {
        private readonly string _filePath;
        private readonly string _brNumberColumnName;
        private readonly string _primaryColumnName;
        private readonly string _secondaryColumnName;

        public URLProvider(IOptions<URLProviderSettings> urlProviderOptions)
        {
            _filePath = urlProviderOptions.Value.FilePath;
            _brNumberColumnName = urlProviderOptions.Value.BRNumberColumnName;
            _primaryColumnName = urlProviderOptions.Value.PrimaryColumnName;
            _secondaryColumnName = urlProviderOptions.Value.SecondaryColumnName;
        }
        public IEnumerable<ProvidedUrls> GetURLs()
        {
            using (var stream = File.Open(_filePath, FileMode.Open, FileAccess.Read))
            {
                // Auto-detect format, supports:
                //  - Binary Excel files (2.0-2003 format; *.xls)
                //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true,
                            TransformValue = (reader, columnIndex, value) =>
                            {
                                if (value is string stringValue)
                                {
                                    return stringValue.Trim();
                                }
                                return value;
                            }
                        }
                    });
                    var firstTable = result.Tables[0];
                    var BRnumberColumnIndex = firstTable.Columns[_brNumberColumnName]?.Ordinal ?? -1;
                    var primaryColumnIndex = firstTable.Columns[_primaryColumnName]?.Ordinal ?? -1;
                    var secondaryColumIndex = firstTable.Columns[_secondaryColumnName]?.Ordinal ?? -1;
                    if (primaryColumnIndex == -1 || secondaryColumIndex == -1)
                    {
                        yield break;
                    }

                    for (int i = 0; i < firstTable.Rows.Count; i++)
                    {
                        var brNumberRow = firstTable.Rows[i][BRnumberColumnIndex].ToString();
                        var primaryURLRow = firstTable.Rows[i][primaryColumnIndex].ToString();
                        var secondaryURLRow = firstTable.Rows[i][secondaryColumIndex].ToString();
                        
                        yield return new ProvidedUrls( brNumberRow ?? string.Empty,
                                                       primaryURLRow ?? string.Empty,
                                                       secondaryURLRow ?? string.Empty);
                    }
                }
            }
        }
    }
}
