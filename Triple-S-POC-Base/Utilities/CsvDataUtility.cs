using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TripleS.Utilities
{
    public static class CsvDataUtility
    {
        // Generic CSV loader
        public static List<T> LoadCsv<T>(string filePath, Func<string[], T> mapFunc)
        {
            var result = new List<T>();
            if (!File.Exists(filePath)) return result;
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var fields = line.Split(',');
                result.Add(mapFunc(fields));
            }
            return result;
        }

        // Generic CSV saver (append mode)
        public static void AppendCsv<T>(string filePath, T record, Func<T, string> toCsvLine)
        {
            var line = toCsvLine(record);
            File.AppendAllText(filePath, line + "\n");
        }

        // Generic CSV overwrite
        public static void SaveCsv<T>(string filePath, IEnumerable<T> records, Func<T, string> toCsvLine)
        {
            var lines = records.Select(toCsvLine);
            File.WriteAllLines(filePath, lines);
        }
    }
}