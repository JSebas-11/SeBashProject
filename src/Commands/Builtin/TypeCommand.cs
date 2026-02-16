using SeBashProject.src.Common;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Drawing;
using SeBashProject.src.Models;
using SeBashProject.src.Utilities.Os;

namespace SeBashProject.src.Commands.Builtin;

internal class TypeCommand : Command {
    public TypeCommand(List<string> args) : base("type", args) 
    {}

    public override async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        foreach (string arg in Args) {
            if (ShellMetadata.IsBuiltin(arg)){
                await TerminalWriter.WriteLineAsync($"{arg} is a shell builtin", stdout);
                continue;
            }
            
            string? execPath = OsInteraction.ExecutablePath(arg);
            if (execPath is not null) {
                await TerminalWriter.WriteLineAsync($"{arg} is {execPath}", stdout);
                continue;
            }

            await TerminalWriter.WriteLineAsync($"{arg}: not found", stdout);
        }
        stdout.Close();
        
        return CmdResult.Ok;
    }
}