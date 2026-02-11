using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.History;
using SeBashProject.src.Models;

namespace SeBashProject.src.Commands.Builtin;

internal sealed class HistoryCommand : Command {
    // --------------------- INIT ---------------------
    private readonly HistoryService _historyService;
    public HistoryCommand(HistoryService historyService, List<string> args) : base("history", args)
        => _historyService = historyService;

    // --------------------- METHODS ---------------------
    public override async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        try {
            if (Args.Count == 0) { // NORMAL EXECUTION NO ARGS
                await PrintLinesAsync(stdout, _historyService.GetLines());
                return CmdResult.Ok;
            }
            
            string arg = Args[0];
            if (arg == "-c") { // CLEAR HISTORY
                // only -c parameter without specific words
                if (Args.Count == 1) _historyService.ClearHistory();
                else _historyService.ClearHistory(Args[1..]);
                
                return CmdResult.Ok;    
            }

            if (arg == "-r") { // APPEND HISTORY FROM FILE
                _historyService.AppendFromFiles(Args[1..]);
                return CmdResult.Ok;    
            }

            if (arg == "-w") { // WRITE HISTORY TO FILE
                _historyService.LinesToFiles(Args[1..]);
                return CmdResult.Ok;    
            }

            if (arg == "-a") { // APPEND HISTORY TO FILE
                _historyService.AppendNewLinesToFiles(Args[1..]);
                return CmdResult.Ok;    
            }

            // HISTORY WITH ESPECIFIED NUMBER OF LINES
            if (Args.Count > 1) {
                await stdout.WriteLineAsync("bash: history: too many arguments");
                return CmdResult.Ok;
            }

            if (!int.TryParse(arg, out int linesNumber)) {
                await stdout.WriteLineAsync($"bash: history: {arg}: numeric argument required");
                return CmdResult.Ok;
            }
            if (linesNumber < 0) {
                await stdout.WriteLineAsync($"bash: history: {linesNumber}: invalid option");
                return CmdResult.Ok;
            }        
            
            await PrintLinesAsync(stdout, _historyService.GetLastNLines(linesNumber));
        }
        finally { stdout.Close(); }
        return CmdResult.Ok;
    }

    // --------------------- PRIVATE METHODS ---------------------
    private async Task PrintLinesAsync(TextWriter stdout, IReadOnlyList<string> lines) {
        int total = _historyService.TotalLines;
        int startIndex = total-lines.Count+1;
        int maxLen = total.ToString().Length;

        foreach (var line in lines) {
            var leftPad = new string(' ', maxLen-startIndex.ToString().Length);
            await stdout.WriteLineAsync($"{leftPad}{startIndex}  {line}");
            startIndex++;
        }
    }
}