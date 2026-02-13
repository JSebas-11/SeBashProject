using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.History;
using SeBashProject.src.External.Abstraction;
using SeBashProject.src.Models;
using SeBashProject.src.Utilities.Os;

namespace SeBashProject.src.Commands.Builtin;

internal sealed class TianCommand : Command {
    // --------------------- INIT ---------------------
    private readonly IGenerativeService _genService;
    private readonly HistoryService _historyService;
    private readonly string Documentation = @"
    Usage: tian [OPTION] ""prompt""

    An AI assistant to help you with command-line tasks.

    Options:
      -e,  --explain       Explain a given command.
      -ef, --exp-file      Explain the content of an external file.
      -g,  --generate      Generate a command based on your prompt.
      -s,  --summarize     Summarize a file (errors, causes, highlights).
      -h,  --history       Generate suggestions using your history and prompt.

    Examples:
      tian -e ""chmod 755 file""
      tian -ef script.sh
      tian -g ""zip all images except png""
      tian -s logs/app.log
      tian -h ""last docker commands I used""";
    public TianCommand(IGenerativeService generativeService, HistoryService historyService, List<string> args) 
        : base("tian", args) 
    {
        _genService = generativeService;
        _historyService = historyService;
    }

    // --------------------- METHODS ---------------------
    public override async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        try {
            string output = null!; 
            if (Args.Count == 0) // No parameters/args specified
                output = Documentation;
            else if (Args.Count == 1)
                output = $"bash: tian: {Args[0]} requires an argument";
            else if (Args.Count > 2)
                output = "bash: tian: too many arguments";
            else {
                string arg = Args[0];
                string value = Args[1];
                output = arg switch {
                    "-e" or "--explain" => await _genService.ExplainCommandAsync(value),
                    "-ef" or "--exp-file" => await HandleExternalFileAsync(value, _genService.ExplainFileAsync),
                    "-g" or "--generate" => await _genService.GenerateCommandAsync(value),
                    "-s" or "--summarize" => await HandleExternalFileAsync(value, _genService.SummarizeAsync),
                    "-h" or "--history" => await HandleHistoryAsync(value),
                    _ => $"bash: tian: {Args[0]} invalid option"
                };
            }

            await stdout.WriteLineAsync(output);
            return CmdResult.Ok;
        }
        finally { stdout.Close(); }
    }

    // --------------------- METHODS ---------------------
    private async Task<string> HandleExternalFileAsync(string filePath, Func<string, Task<string>> function) {
        string? contentFile = await OsInteraction.ContentFileAsync(filePath);

        return string.IsNullOrWhiteSpace(contentFile)
            ? $"bash: tian: {filePath} does not exist or it is empty"
            : await function.Invoke(contentFile);
    }

    private async Task<string> HandleHistoryAsync(string prompt)
        => _historyService.TotalLines == 0 
            ? "bash: tian: not context, history is empty"
            : await _genService.HistoryBasedAsync(_historyService.GetLines(), prompt);
}