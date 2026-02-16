using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Drawing;
using SeBashProject.src.Core.History;
using SeBashProject.src.Models;

namespace SeBashProject.src.Commands.Builtin;

internal sealed class HistoryCommand : Command {
    // --------------------- INIT ---------------------
    private readonly HistoryService _historyService;
    private readonly string _documentation = @"
    Usage: history [OPTION]

    Display or manipulate the command history list.

    Options:
      none                    Display the entire history list.
      <number>                Display the last <number> entries of the history list.
      -h                      Show this help documentation.
      -c                      Clear all entries from the history list.
      -c <cmd>                Remove all occurrences of the specified command from the history list.
      -r [filePath]           Read history entries from the file and append them to the current history list.
      -w [filePath]           Write the current history list to the specified file.
      -a [filePath]           Append history entries from this session to the specified file.

    Examples:
      history 20
      history -c docker
      history -r ~/.history
      history -w backup.txt";

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
            if (arg == "-h") {
                await TerminalWriter.WriteLineAsync(_documentation, stdout, TerminalStyles.Panel, "Documentation");
                return CmdResult.Ok;    
            }
            
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
                await TerminalWriter.WriteLineAsync("bash: history: too many arguments", stderr, TerminalStyles.Error);
                return CmdResult.Ok;
            }

            if (!int.TryParse(arg, out int linesNumber)) {
                await TerminalWriter.WriteLineAsync($"bash: history: {arg}: numeric argument required", stderr, TerminalStyles.Error);
                return CmdResult.Ok;
            }
            if (linesNumber < 0) {
                await TerminalWriter.WriteLineAsync($"bash: history: {linesNumber}: invalid option", stderr, TerminalStyles.Error);
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
            await TerminalWriter.WriteLineAsync($"{leftPad}{startIndex}  {line}", stdout);
            startIndex++;
        }
    }
}