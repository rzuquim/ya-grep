using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using YAGrep;

namespace Grep.Test {
    public class LineReaderTest {
        [Test]
        public async Task can_read_lines() {
            using var fileStream = File.OpenRead("./Data/basic_text.txt");
            using var reader = new StreamReader(fileStream);
            var lineReader = new LineReader(reader, GrepOptions.Default);

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.ReadLine()).Valid())
                lines.Add(line.ToString());

            Assert.That(lines, Is.EquivalentTo(new [] {
                "Roses are red,",
                "Violets are blue,",
                "Sugar is sweet,",
                "And so are you."
            }));
        }

        [Test]
        public async Task supports_lines_bigger_than_the_buffer() {
            using var fileStream = File.OpenRead("./Data/basic_text.txt");
            using var reader = new StreamReader(fileStream);
            var lineReader = new LineReader(reader, new GrepOptions(bufferSize: 10));

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.ReadLine()).Valid())
                lines.Add(line.ToString());

            Assert.That(lines, Is.EquivalentTo(new [] {
                "Roses are red,",
                "Violets are blue,",
                "Sugar is sweet,",
                "And so are you."
            }));
        }
    }
}
