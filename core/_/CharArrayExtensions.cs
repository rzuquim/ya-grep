namespace YAGrep {
    public static class CharArrayExtensions {
        public static int IndexOf(this char[] haystack, char needle, int startIndex, int count) {
            var limit = startIndex + count;
            for (var i = startIndex; i < limit; i++) {
                if (haystack[i] != needle) continue;
                return i;
            }

            return -1;
        }
    }
}
