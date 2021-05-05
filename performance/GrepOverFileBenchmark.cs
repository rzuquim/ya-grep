using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using YAGrep;

namespace Grep.Performance {
    [MemoryDiagnoser]
    public class GrepOverFileBenchmark {
        [GlobalSetup]
        public void DeflateDataFiles() {
            if (!Directory.Exists(_dataSource))
                Directory.CreateDirectory(_dataSource);

            foreach (var zip in Directory.GetFiles(@".\Data\", "*.zip")) {
                using var zipFile = ZipFile.OpenRead(zip);
                var zipEntry = zipFile.Entries.First();
                var targetFile = $@"{_dataSource}{Path.GetFileNameWithoutExtension(zip)}";

                if (File.Exists(targetFile)) {
                    Console.WriteLine("=============================================");
                    Console.WriteLine($"Already deflated {targetFile}");
                    Console.WriteLine("=============================================");
                    continue;
                }
                Console.WriteLine("=============================================");
                Console.WriteLine($"Deflating {targetFile}");
                Console.WriteLine("=============================================");
                zipEntry.ExtractToFile(targetFile);
            }
        }

        [Benchmark]
        [Arguments("small")]
        [Arguments("medium")]
        [Arguments("big")]
        // [Arguments("huge")]
        public async Task MatchingSearch(string fileName) {
            var filePath = @$"{_dataSource}{fileName}";
            var (smallMatch, bigMatch) = _matches[fileName];

            var resultCount = 0;
            await filePath.Grep(smallMatch, _ => { resultCount++; return true; });
            await filePath.Grep(bigMatch, _ => { resultCount++; return true; });
            _sanityCheck[fileName] = resultCount;
        }

        [GlobalCleanup]
        public void PrintSanityCheck() {
            foreach (var (file, count) in _sanityCheck) {
                Console.WriteLine("=============================================");
                Console.WriteLine($"File: {file} Result count: {count}");
                Console.WriteLine("=============================================");
            }
        }

        private static readonly Dictionary<string, int> _sanityCheck = new();

        private const string _dataSource = @".\Data\Deflated\";

        private static readonly Dictionary<string, (string smallMatch, string bigMatch)> _matches = new() {
            { "small", ("original", "in") },
            { "medium", ("mist", "God") },
            { "big", ("whoops", "username") },
            { "huge", ("bvmf.018.02", "10=043") },
        };
    }
}
