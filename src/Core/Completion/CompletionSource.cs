using SeBashProject.src.Common;
using SeBashProject.src.Utilities.Os;

namespace SeBashProject.src.Core.Completion;

internal static class CompletionSource {
    public static HashSet<string> BuiltinMatches(string token)
        => [.. ShellMetadata.Builtins.Where(cmd => cmd.StartsWith(token))];

    public static HashSet<string> ExecutableMatches(string token) {
        HashSet<string> matches = [];

        foreach (string file in OsInteraction.GetPathFiles()) {
            string name = Path.GetFileName(file);

            if (name.StartsWith(token)) matches.Add(name);
        }

        return matches;
    }
}