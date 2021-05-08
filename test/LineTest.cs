using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using YAGrep;

namespace Grep.Test {
    public class LineTest {
        [Test]
        public void can_read_text_on_multiline_buffer() {
            var completeFile = File.ReadAllText("./Data/text.txt").ToCharArray();

            var line = new Line();
            line.StartChunk(completeFile, completeFile.Length);
            Assert.That(line.AsString(), Is.EqualTo("April is the cruellest month, breeding"));
            Assert.That(line.Length, Is.EqualTo(38));
            Assert.That(line.IndexOf('i', startIndex: 0, limit: 38), Is.EqualTo(3));
            Assert.That(line.IndexOf('i', startIndex: 4, limit: 38), Is.EqualTo(6));
            Assert.That(line.IndexOf('i', startIndex: 7, limit: 38), Is.EqualTo(35));

            line.NextLine();
            Assert.That(line.AsString(), Is.EqualTo("Lilacs out of the dead land, mixing"));
            Assert.That(line.Length, Is.EqualTo(35));
            Assert.That(line.IndexOf('i', startIndex: 0, limit: 35), Is.EqualTo(1));
            Assert.That(line.IndexOf('i', startIndex: 2, limit: 35), Is.EqualTo(30));
            Assert.That(line.IndexOf('i', startIndex: 31, limit: 35), Is.EqualTo(32));
        }

        [Test]
        public void can_count_lines() {
            var completeFile = File.ReadAllText("./Data/text.txt").ToCharArray();
            var line = new Line();
            line.StartChunk(completeFile, completeFile.Length);

            while (line.Valid()) {
                a.Add(line.AsString());
                validLinesCount++;
                line.NextLine();
            }

            Assert.That(validLinesCount, Is.EqualTo(17));


        }
    }
}
