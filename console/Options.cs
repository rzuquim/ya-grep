using System.CommandLine;
using System.IO;

namespace YAGrep {
    public static class Options {
        public static Option<string> Regexp { get; } = new RegexpOption();
        public static Option<FileInfo> File { get; } = new FileOption().ExistingOnly();

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
    }
}
