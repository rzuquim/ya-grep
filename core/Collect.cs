using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace YAGrep {
    public static class GrepAndCollect {
        public static async Task<IEnumerable<GrepResult>> Grep(
                this Stream haystack, string needle, int top = -1, GrepOptions? options = null) {
            var result = new List<GrepResult>();

            bool collect(GrepResult match) {
                result.Add(match);
                return top < 0 || result.Count < top;
            }

            await haystack.Grep(needle, collect, options);
            return result;
        }
    }
}
