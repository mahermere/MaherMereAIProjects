namespace Triple_S_Maui_AEP.Utilities
{
    /// <summary>
    /// Utility class for CSV data operations
    /// </summary>
    public static class CsvDataUtility
    {
        public static string[] ParseCsvLine(string line)
        {
            var fields = new List<string>();
            var currentField = new System.Text.StringBuilder();
            var insideQuotes = false;

            foreach (var c in line)
            {
                if (c == '"')
                {
                    insideQuotes = !insideQuotes;
                }
                else if (c == ',' && !insideQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            fields.Add(currentField.ToString());
            return fields.ToArray();
        }

        public static List<string[]> ParseCsv(string csvContent)
        {
            var lines = csvContent.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var records = new List<string[]>();

            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    records.Add(ParseCsvLine(line));
                }
            }

            return records;
        }
    }
}
