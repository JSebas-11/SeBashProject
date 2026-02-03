using SeBashProject.src.Common;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models;
using SeBashProject.src.Utilities.Os;

namespace SeBashProject.src.Commands.Builtin;

internal class TypeCommand : Command {
    public TypeCommand(List<string> args) : base("type", args) 
    {}

    public override async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        foreach (string arg in Args) {
            if (ShellMetadata.IsBuiltin(arg)){
                await stdout.WriteLineAsync($"{arg} is a shell builtin");
                continue;
            }
            
            string? execPath = OsInteraction.ExecutablePath(arg);
            if (execPath is not null) {
                await stdout.WriteLineAsync($"{arg} is {execPath}");
                continue;
            }

            await stdout.WriteLineAsync($"{arg}: not found");
        }
        stdout.Close();
        
        return CmdResult.Ok;
    }
}