using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models;

namespace SeBashProject.src.Commands.Builtin;

internal class ExitCommand : Command {
    public ExitCommand() : base("exit", []) { }

    public override async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        _ = Task.Run(() => Environment.Exit(0));
        return CmdResult.Exit;
    }
}