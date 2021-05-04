using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using YAGrep;

namespace Grep.Test {
    public class StringMatchTest {
        [Test]
        public async Task can_search_text() {
            Assert.That(await File.OpenRead("./Data/basic_text.txt").Grep("red"),
                Is.EquivalentTo(new [] { "Roses are red," }));
            Assert.That(await File.OpenRead("./Data/basic_text.txt").Grep("blue"),
                Is.EquivalentTo(new [] { "Violets are blue," }));
        }
    }
}
