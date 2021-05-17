using NUnit.Framework;
using YAGrep;

namespace Grep.Test {
    public class MatchFunctionStringContainsTest {
        [Test]
        public void can_find_needle_in_line() {
            var matchFn = MatchFunction.For("month", GrepOptions.Default);
            var line = new Line("April is the cruellest month, breeding");

            var result = matchFn(line, 0);

            Assert.That(result.IsMatch);
            Assert.That(result.Line.AsString(), Is.EqualTo("April is the cruellest month, breeding"));
            Assert.That(result.LineNumber, Is.EqualTo(1));
            Assert.That(result.MatchStart, Is.EqualTo(23));
            Assert.That(result.MatchEnd, Is.EqualTo(27));
        }


        [Test]
        public void wont_find_needle_if_not_there() {
            var matchFn = MatchFunction.For("month", GrepOptions.Default);
            var line = new Line("Lilacs out of the dead land, mixing");

            var result = matchFn(line, 0);

            Assert.That(!result.IsMatch);
        }
    }
}
