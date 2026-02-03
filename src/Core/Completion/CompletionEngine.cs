using System.Text;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models;

namespace SeBashProject.src.Core.Completion;

internal static class CompletionEngine {
    public static CompletionResult Complete(string token) {
        HashSet<string> matches = CompletionSource.BuiltinMatches(token);
        matches.UnionWith(CompletionSource.ExecutableMatches(token));

        List<string> orderedMatches = [.. matches.OrderBy(m => m)];

        CompletionType type = orderedMatches.Count switch {
            0 => CompletionType.NoMatch,
            1 => CompletionType.OneMatch,
            _ => CompletionType.MultipleMatch
        };
        
        return new CompletionResult(type, orderedMatches);
    }

    public static string? LongestCommonPrefix(List<string> matches) {
        if (matches.Count <= 1) return null;

        string shortestMatch = matches.MinBy(mtc => mtc.Length)!;
        var lcp = new StringBuilder();

        for (int i = 0; i < shortestMatch.Length; i++) {
            char currentChar = shortestMatch[i];
            
            foreach (var match in matches) {
                if (match[i] != currentChar) 
                    return lcp.Length > 0 ? lcp.ToString() : null;
            }

            lcp.Append(currentChar);
        }

        return lcp.Length > 0 ? lcp.ToString() : null;
    }
}