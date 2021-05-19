using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YAGrep;

namespace Grep.Test {
    public class LineReaderTest {
        [Test]
        public async Task can_read_single_line() {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_singleLine));
            using var reader = new StreamReader(stream);

            var lineReader = new LineReader(reader, GrepOptions.Default);

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.NextLine()).Valid())
                lines.Add(line.AsString());

            Assert.That(lines, Is.EquivalentTo(new[] { "Roses are red," }));
        }

        [Test]
        public async Task can_read_multiple_lines() {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_multipleLines));
            using var reader = new StreamReader(stream);

            var lineReader = new LineReader(reader, GrepOptions.Default);

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.NextLine()).Valid())
                lines.Add(line.AsString());

            Assert.That(lines, Is.EquivalentTo(new[] {
                "Roses are red,",
                "Violets are blue,",
                "Sugar is sweet,",
                "And so are you."
            }));
        }

        [Test]
        public async Task can_read_multiple_lines_with_first_line_empty() {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_multipleLinesWithFirstLineBreak));
            using var reader = new StreamReader(stream);

            var lineReader = new LineReader(reader, GrepOptions.Default);

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.NextLine()).Valid())
                lines.Add(line.AsString());

            Assert.That(lines, Is.EquivalentTo(new[] {
                "",
                "Roses are red,",
                "Violets are blue,",
                "Sugar is sweet,",
                "And so are you."
            }));
        }

        [Test, Ignore("Since we are using the '\\n' as delimiter, this is not a supported scenario")]
        public async Task can_read_multiple_lines_with_last_line_empty() {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_multipleLinesWithLastLineBreak));
            using var reader = new StreamReader(stream);

            var lineReader = new LineReader(reader, GrepOptions.Default);

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.NextLine()).Valid())
                lines.Add(line.AsString());

            Assert.That(lines, Is.EquivalentTo(new[] {
                "Roses are red,",
                "Violets are blue,",
                "Sugar is sweet,",
                "And so are you.",
                ""
            }));
        }

        [Test]
        public async Task can_read_multiple_lines_with_empty_line_on_the_mix() {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_multipleLinesWithEmptyLine));
            using var reader = new StreamReader(stream);

            var lineReader = new LineReader(reader, GrepOptions.Default);

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.NextLine()).Valid())
                lines.Add(line.AsString());

            Assert.That(lines, Is.EquivalentTo(new[] {
                "Roses are red,",
                "",
                "",
                "Violets are blue,",
                "Sugar is sweet,",
                "",
                "And so are you."
            }));
        }

        [Test]
        public async Task can_read_multiple_lines_on_windows() {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_multipleLinesWithWindowsLineBreaks));
            using var reader = new StreamReader(stream);

            var lineReader = new LineReader(reader, GrepOptions.Default);

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.NextLine()).Valid())
                lines.Add(line.AsString());

            Assert.That(lines, Is.EquivalentTo(new[] {
                "Roses are red,",
                "Violets are blue,",
                "Sugar is sweet,",
                "And so are you."
            }));
        }

        [Test]
        public async Task can_read_empty() {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_empty));
            using var reader = new StreamReader(stream);

            var lineReader = new LineReader(reader, GrepOptions.Default);

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.NextLine()).Valid())
                lines.Add(line.AsString());

            Assert.That(lines, Is.Empty);
        }

        [Test]
        public async Task supports_lines_bigger_than_the_buffer() {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_multipleLines));
            using var reader = new StreamReader(stream);

            var lineReader = new LineReader(reader, new GrepOptions(bufferSize: 10));

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.NextLine()).Valid())
                lines.Add(line.AsString());

            Assert.That(lines, Is.EquivalentTo(new[] {
                "Roses are red,",
                "Violets are blue,",
                "Sugar is sweet,",
                "And so are you."
            }));
        }

        [Test]
        public async Task can_read_multiple_lines_with_trim() {
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(_multipleLinesNeedingTrim));
            using var reader = new StreamReader(stream);

            var lineReader = new LineReader(reader, new GrepOptions(trim: true));

            Line line;
            var lines = new List<string>();
            while ((line = await lineReader.NextLine()).Valid())
                lines.Add(line.AsString());

            Assert.That(lines, Is.EquivalentTo(new[] {
                "Roses are red,",
                "",
                "Violets are blue,",
                "Sugar is sweet,",
                "",
                "And so are you."
            }));
        }

        #region possibilities
        private readonly char[] _empty = "".ToCharArray();
        private readonly char[] _singleLine = "Roses are red,".ToCharArray();

        private readonly char[] _multipleLines = ("Roses are red," + '\n' +
                                                  "Violets are blue," + '\n' +
                                                  "Sugar is sweet," + '\n' +
                                                  "And so are you.").ToCharArray();

        private readonly char[] _multipleLinesWithFirstLineBreak =
            ('\n' +
             "Roses are red," + '\n' +
             "Violets are blue," + '\n' +
             "Sugar is sweet," + '\n' +
             "And so are you.").ToCharArray();

        private readonly char[] _multipleLinesWithLastLineBreak =
            ("Roses are red," + '\n' +
             "Violets are blue," + '\n' +
             "Sugar is sweet," + '\n' +
             "And so are you." + '\n').ToCharArray();

        private readonly char[] _multipleLinesNeedingTrim =
            ("Roses are red,  " + '\n' +
             '\n' +
             "   Violets are blue, " + '\n' +
             " Sugar is sweet,         " + '\n' +
             "    " + '\n' +
             "    And so are you.   ").ToCharArray();

        private readonly char[] _multipleLinesWithEmptyLine =
            ("Roses are red," + '\n' +
             '\n' +
             '\n' +
             "Violets are blue," + '\n' +
             '\n' +
             "Sugar is sweet," + '\n' +
             "And so are you." + '\n').ToCharArray();

        private readonly char[] _multipleLinesWithWindowsLineBreaks =
            ("Roses are red," + '\r' + '\n' +
             "Violets are blue," + '\r' + '\n' +
             "Sugar is sweet," + '\r' + '\n' +
             "And so are you.").ToCharArray();
        #endregion
    }
}
