using System.CommandLine;
using System.IO;

namespace YAGrep {
    public static class Options {
        public static Option<string> Regexp { get; } = new RegexpOption();
        public static Option<FileInfo> File { get; } = new FileOption().ExistingOnly();
        public static Option<bool> PrintStatistics { get; } = new PrintStatisticsFlag();
        public static Option<bool> Trim { get; } = new TrimFlag();
        public static Option<int> MaxCount { get; } = new MaxCountOption();

        public class RegexpOption : Option<string> {
            public RegexpOption() : base("--regexp") {
                Description = "The  PATTERNS to be searched.";
                IsRequired = true;
                AddAlias("-e");
            }
        }

        public class FileOption : Option<FileInfo> {
            public FileOption() : base("--file") {
                Description = "Source file where to find PATTERNS";
                IsRequired = true;
                AddAlias("-f");
            }
        }

        public class PrintStatisticsFlag : Option<bool> {
            public PrintStatisticsFlag() : base("--statistics") {
                Description = "Prints total line count and elapsed time after search results";
                AddAlias("-s");
            }
        }

        public class TrimFlag : Option<bool> {
            public TrimFlag() : base("--trim") {
                Description = "Should trim lines before search.";
            }
        }

        public class MaxCountOption : Option<int> {
            public MaxCountOption() : base("--max-count") {
                Description = "Stop reading a file after NUM matching lines.";
                AddAlias("-m");
            }
        }
    }
}
