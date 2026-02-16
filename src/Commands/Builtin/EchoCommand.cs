using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Drawing;
using SeBashProject.src.Models;

namespace SeBashProject.src.Commands.Builtin;

internal sealed class EchoCommand : Command {
    public EchoCommand(List<string> args) : base("echo", args) 
    {}

    public override async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        await TerminalWriter.WriteLineAsync(string.Join(' ', Args), stdout);
        stdout.Close();
        return CmdResult.Ok;
    }
}