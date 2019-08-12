using Mono.Options;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LogFileParser
{
    class Program
    {
        static void Main(string[] args)
        {           
            string fileName = "";
            string searchTerm = "";
            var shouldShowHelp = false;
            var shouldCopyToClipboard = false;
            var occurrencesText = "occurrences";

            var options = new OptionSet {
                { "f|filename=", "The full path of the filename to parse.", f => fileName = f },
                { "s|searchTerm=", "The text that should occur in the particular line", s => searchTerm = s },
                { "o|occurrences=", "Replaces the text \"occurrences\" to the desired value, e.g., \"errors\".", o => occurrencesText = o },
                { "c|clipboard", "Copies the contents of the parsed output to clipboard", c => shouldCopyToClipboard = c != null },
                { "h|help", "show this message and exit", h => shouldShowHelp = h != null }
            };
            options.Parse(args);

            if (!shouldShowHelp)
            {
                string result = SummarizeLogContents(fileName, searchTerm, occurrencesText);

                Console.WriteLine(result);
                Console.WriteLine();

                if (shouldCopyToClipboard)
                {
                    TextCopy.Clipboard.SetText(result);
                    Console.WriteLine("Contents copied to clipboard...");
                }
                
                Console.ReadLine();
            }

            Console.WriteLine("Error Log Parser");
            Console.WriteLine();
            Console.WriteLine("Options:");
            options.WriteOptionDescriptions(Console.Out);
            Console.ReadLine();
        }

        private static string SummarizeLogContents(string fileName, string searchTerm, string occurrencesText)
        {
            var fileText = File.ReadAllText(fileName);
            var fileLines = fileText.Split(Environment.NewLine);

            var occurrences = fileLines.Where(x => x.Contains(searchTerm));
            var occurrenceGroups = occurrences.GroupBy(z => z).OrderByDescending(x => x.Count());

            var builder = new StringBuilder();

            builder.AppendLine($"{occurrences.Count()} {occurrencesText} found with the text \"{searchTerm}\".");
            occurrenceGroups.ToList()
                .ForEach(x => builder.AppendLine($"{x.Count()} occurrences:{x.Key}"));

            var result = builder.ToString();
            return result;
        }
    }
}
