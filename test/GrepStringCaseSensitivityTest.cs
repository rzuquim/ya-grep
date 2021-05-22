using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using YAGrep;
using static YAGrep.Case;

namespace Grep.Test {
    public class GrepStringCaseSensitivityTest {
        [Test]
        public async Task can_find_single_match_case_insensitive() {
            using var reader = GetReader();
            Assert.That((await GetReader().Grep("RED", new GrepOptions(@case: Insensitive))),
                Is.EquivalentTo(
                    new [] {
                        GrepResultSuccess("Roses are red,", lineNumber: 1, matchStart: 10, matchEnd: 12)
                    }
            ));

            reader.Position = 0; //resetting stream

            Assert.That(await reader.Grep("blue"),
                Is.EquivalentTo(
                    new [] {
                        GrepResultSuccess("Violets are blue,", lineNumber: 2, matchStart: 12, matchEnd: 15)
                    }
            ));
        }


        [Test]
        public async Task can_find_multiple_matches_case_insensitive() {
            using var reader = GetReader();
            foreach (var term in new [] { "are", "aRe", "ARE"}) {
                Assert.That(await reader.Grep(term),
                    Is.EquivalentTo(
                        new [] {
                            GrepResultSuccess("Roses are red,", lineNumber: 1, matchStart: 6, matchEnd: 8),
                            GrepResultSuccess("Violets are blue,", lineNumber: 2, matchStart: 8, matchEnd: 10),
                            GrepResultSuccess("And so are you.", lineNumber: 4, matchStart: 7, matchEnd: 9)
                        }
                ));
            }
        }

        [Test]
        public async Task can_find_multiple_matches_with_trim_case_insensitive() {
            using var reader = GetReader(_textNeedingTrim);
            foreach (var term in new [] { "are", "aRe", "ARE"}) {
                Assert.That(await reader.Grep(term, options: new GrepOptions(trim: true)),
                    Is.EquivalentTo(
                        new [] {
                            GrepResultSuccess("Roses are red,", lineNumber: 1, matchStart: 6, matchEnd: 8),
                            GrepResultSuccess("Violets are blue,", lineNumber: 2, matchStart: 8, matchEnd: 10),
                            GrepResultSuccess("And so are you.", lineNumber: 4, matchStart: 7, matchEnd: 9)
                        }
                ));
            }
        }

        #region test environment
        private Stream GetReader(char[] text = null) => new MemoryStream(Encoding.UTF8.GetBytes(text ?? _defaultText));

        private static GrepResult GrepResultSuccess(
                string expectedText, int lineNumber, int matchStart, int matchEnd) =>
            GrepResult.Success(new Line(expectedText), lineNumber, matchStart, matchEnd);

        private readonly char[] _defaultText =
            ("Roses are red," + '\r' + '\n' +
             "Violets are blue," + '\r' + '\n' +
             "Sugar is sweet," + '\r' + '\n' +
             "And so are you.").ToCharArray();

        private readonly char[] _textNeedingTrim =
            ("             Roses are red, " + '\r' + '\n' +
             "   Violets are blue," + '\r' + '\n' +
             "Sugar is sweet,          " + '\r' + '\n' +
             "And so are you.").ToCharArray();
        #endregion
    }
}
