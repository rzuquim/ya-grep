using System.Threading.Tasks;
using NUnit.Framework;
using YAGrep;

namespace Grep.Test {
    public class StringMatchTest {
        [Test]
        public async Task can_find_single_match() {
            const string file = "./Data/basic_text.txt";
            Assert.That(await file.Grep("red"),
                Is.EquivalentTo(
                    new [] {
                        GrepResult.Success("Roses are red,", lineNumber: 1, matchStart: 10, matchEnd: 12)
                    }
            ));

            Assert.That(await file.Grep("blue"),
                Is.EquivalentTo(
                    new [] {
                        GrepResult.Success("Violets are blue,", lineNumber: 2, matchStart: 12, matchEnd: 15)
                    }
            ));
        }

        [Test]
        public async Task can_find_multiple_matches() {
            const string file = "./Data/basic_text.txt";

            Assert.That(await file.Grep("are"),
                Is.EquivalentTo(
                    new [] {
                        GrepResult.Success("Roses are red,", lineNumber: 1, matchStart: 6, matchEnd: 8),
                        GrepResult.Success("Violets are blue,", lineNumber: 2, matchStart: 8, matchEnd: 10),
                        GrepResult.Success("And so are you.", lineNumber: 4, matchStart: 7, matchEnd: 9)
                    }
            ));
        }

        [Test]
        public async Task supports_empty_result() {
            const string file = "./Data/basic_text.txt";
            Assert.That(await file.Grep("yellow"), Is.Empty);
        }
    }
}
