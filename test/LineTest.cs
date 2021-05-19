using NUnit.Framework;
using YAGrep;

namespace Grep.Test {
    public class LineTest {
        [Test]
        public void can_setup_lines() {
            var singleLine = new Line(_singleLine).Update(startIndex: 0, length: 26);

            Assert.That(singleLine.Length, Is.EqualTo(26));
            Assert.That(singleLine.AsString(), Is.EqualTo("Lorem ipsum dolor sit amet"));

            var multipleLines = new Line(_multipleLines).Update(startIndex: 0, length: 26);

            Assert.That(multipleLines.Length, Is.EqualTo(26));
            Assert.That(multipleLines.AsString(), Is.EqualTo("Lorem ipsum dolor sit amet"));

            multipleLines = new Line(_multipleLines).Update(startIndex: 27, length: 27);
            Assert.That(multipleLines.Length, Is.EqualTo(27));
            Assert.That(multipleLines.AsString(), Is.EqualTo("Consectetur adipiscing elit"));

            multipleLines = new Line(_multipleLines).Update(startIndex: 55, length: 32);
            Assert.That(multipleLines.Length, Is.EqualTo(32));
            Assert.That(multipleLines.AsString(), Is.EqualTo("Sed do eiusmod tempor incididunt"));
        }

        [Test]
        public void can_create_line_snapshot() {
            var multipleLines = new Line(_multipleLines).Update(startIndex: 27, length: 27);
            var snapshot = multipleLines.Snapshot();
            Assert.That(multipleLines, Is.Not.EqualTo(snapshot));
            Assert.That(snapshot.AsString(), Is.EqualTo("Consectetur adipiscing elit"));
        }

        [Test]
        public void lenght_greater_than_buffer_is_invalid() {
            var invalidLine = new Line(_singleLine).Update(startIndex: 0, length: 27);

            Assert.That(invalidLine.Valid(), Is.False);
            Assert.That(invalidLine.Length, Is.EqualTo(-1));
            Assert.That(invalidLine.IndexOf('o', startSearch: 0, limit: 27), Is.EqualTo(-1));
        }

        [Test]
        public void can_search_chars() {
            var line = new Line(_multipleLines).Update(startIndex: 27, length: 27);
            Assume.That(line.AsString(), Is.EqualTo("Consectetur adipiscing elit"));

            Assert.That(line.IndexOf('e', 0, 4), Is.EqualTo(-1));
            Assert.That(line.IndexOf('e', 0, 10), Is.EqualTo(4));
            Assert.That(line.IndexOf('e', 5, 10), Is.EqualTo(7));
            Assert.That(line.IndexOf('e', 22, 4), Is.EqualTo(23));
        }

        [Test]
        public void can_reference_chars() {
            var line = new Line(_multipleLines).Update(startIndex: 27, length: 27);
            Assume.That(line.AsString(), Is.EqualTo("Consectetur adipiscing elit"));

            Assert.That(line[2], Is.EqualTo('n'));
            Assert.That(line[10], Is.EqualTo('r'));
        }

        [Test]
        public void should_enforce_line_boundaries_on_char_search() {
            var line = new Line(_multipleLines).Update(startIndex: 27, length: 27);
            Assume.That(line.AsString(), Is.EqualTo("Consectetur adipiscing elit"));

            Assert.That(line.IndexOf('e', 24, 50), Is.EqualTo(-1));
        }

        #region possibilities
        private readonly char[] _singleLine = "Lorem ipsum dolor sit amet".ToCharArray();
        private readonly char[] _multipleLines =
            ("Lorem ipsum dolor sit amet" + '\n' +
             "Consectetur adipiscing elit" + '\n' +
             "Sed do eiusmod tempor incididunt").ToCharArray();
        #endregion
    }
}
