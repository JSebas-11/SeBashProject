using SeBashProject.src.Common.Enums;

namespace SeBashProject.src.Models;

internal class CompletionResult {
    public CompletionType CompletionType { get; }
    public List<string> Matches { get; }

    public CompletionResult(CompletionType type, List<string> matches) {
        CompletionType = type;
        Matches = matches;
    }
}