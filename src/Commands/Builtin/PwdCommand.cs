using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Drawing;
using SeBashProject.src.Models;
using SeBashProject.src.Utilities.Os;

namespace SeBashProject.src.Commands.Builtin;

internal class PwdCommand : Command {
    public PwdCommand() : base("pwd", []) { }

    public override async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        await TerminalWriter.WriteLineAsync(OsInteraction.GetCurrentDirectory(), stdout);
        stdout.Close();
        return CmdResult.Ok;
    }
}