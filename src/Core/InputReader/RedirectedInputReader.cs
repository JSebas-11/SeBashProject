using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Completion;
using SeBashProject.src.Models;

namespace SeBashProject.src.Core.InputReader;

internal class RedirectedInputReader : IInputReader {
    // --------------------- INIT ---------------------
    private string? line;

    // --------------------- METHODS ---------------------
    public string? ReadLine() {
        line = Console.ReadLine(); 

        if (line is null) return null;
        
        Console.Write("$ ");
        HandleCompletion();
        Console.Write(line);
        
        return line;
    }

    // --------------------- INTERNAL METHODS ---------------------
    private void HandleCompletion() {
        string? token = GetCompletionToken();
        if (token is null) return;

        CompletionResult result = CompletionEngine.Complete(token);

        if (result.CompletionType == CompletionType.OneMatch)
            ReplaceToken(result.Matches[0]);

    }

    private string? GetCompletionToken() {
        int tabIndex = line!.IndexOf('\t'); 
        if (tabIndex == -1) return null;

        //Extract substring of line since beggining until TabIndex
        string token = line[..tabIndex];
        int lastSpace = token!.LastIndexOf(' ');

        return lastSpace == -1 ? token : token[(lastSpace+1)..];
    }

    private void ReplaceToken(string newToken) {
        string beforeTab = line![..line!.IndexOf('\t')];
        int lastSpace = beforeTab!.LastIndexOf(' ');

        string prefix = lastSpace == -1 ? "" : beforeTab[..(lastSpace+1)];

        line = $"{prefix}{newToken} ";
    }
}