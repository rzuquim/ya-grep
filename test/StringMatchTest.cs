using System.Threading.Tasks;
using NUnit.Framework;
using YAGrep;

namespace Grep.Test {
    public class StringMatchTest {
        [Test]
        public async Task can_find_single_match() {
            const string file = "./Data/basic_text.txt";
            Assert.That((await file.Grep("red")),
                Is.EquivalentTo(
                    new [] {
                        GrepResultSuccess("Roses are red,", lineNumber: 1, matchStart: 10, matchEnd: 12)
                    }
            ));

            Assert.That(await file.Grep("blue"),
                Is.EquivalentTo(
                    new [] {
                        GrepResultSuccess("Violets are blue,", lineNumber: 2, matchStart: 12, matchEnd: 15)
                    }
            ));
        }

        [Test]
        public async Task can_find_multiple_matches() {
            const string file = "./Data/basic_text.txt";

            Assert.That(await file.Grep("are"),
                Is.EquivalentTo(
                    new [] {
                        GrepResultSuccess("Roses are red,", lineNumber: 1, matchStart: 6, matchEnd: 8),
                        GrepResultSuccess("Violets are blue,", lineNumber: 2, matchStart: 8, matchEnd: 10),
                        GrepResultSuccess("And so are you.", lineNumber: 4, matchStart: 7, matchEnd: 9)
                    }
            ));
        }

        [Test]
        public async Task supports_empty_result() {
            const string file = "./Data/basic_text.txt";
            Assert.That(await file.Grep("yellow"), Is.Empty);
        }

        private static GrepResult GrepResultSuccess(string expectedText, int lineNumber, int matchStart, int matchEnd) {
            var line = new Line();
            line.StartChunk(expectedText.ToCharArray(), expectedText.Length);
            return GrepResult.Success(line, lineNumber, matchStart, matchEnd);
        }
    }
}
