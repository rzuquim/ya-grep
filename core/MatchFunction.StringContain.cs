using System;

namespace YAGrep {
    // based on: https://www.codeproject.com/Articles/43726/Optimizing-string-operations-in-C
    // TODO: case-insensitive
    internal partial class MatchFunction {
        private static Func<string, int, GrepResult> NaiveStringContains(string needle, GrepOptions options) =>
            (line, lineIndex) => {
                var limit = line.Length - needle.Length + 1;
                if (limit < 1) return GrepResult.Failure(line);

                // Store the first 2 characters of the needle
                var c0 = needle[0];
                var c1 = needle[1];

                // Find the first occurrence of the first character
                var possibleMatchStart = line.IndexOf(c0, startIndex: 0, limit);
                while (possibleMatchStart != -1) {
                    // Check if the following character is the same like the 2nd character of the needle
                    if (line[possibleMatchStart + 1] != c1) {
                        possibleMatchStart = line.IndexOf(c0, ++possibleMatchStart, limit - possibleMatchStart);
                        continue;
                    }

                    var found = true;
                    // Looking for the rest of the needle (starting with the 3rd character)
                    for (var i = 2; i < needle.Length; i++)
                        if (line[possibleMatchStart + i] != needle[i]) { found = false; break; }
                    // If the whole word was found, return its index, otherwise try again
                    if (found)
                        return GrepResult.Success(line, lineIndex + 1,
                                                  possibleMatchStart, possibleMatchStart + needle.Length - 1);
                    possibleMatchStart = line.IndexOf(c0, ++possibleMatchStart, limit - possibleMatchStart);
                }

                return GrepResult.Failure(line);
            };
    }
}
