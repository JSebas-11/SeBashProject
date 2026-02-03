using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models;

namespace SeBashProject.src.Commands.Builtin;

internal sealed class EchoCommand : Command {
    public EchoCommand(List<string> args) : base("echo", args) 
    {}

    public override async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        await stdout.WriteLineAsync(string.Join(' ', Args));
        stdout.Close();
        return CmdResult.Ok;
    }
}