using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Execution;

namespace SeBashProject.src.Models;

internal class Command : IExecutable {
    // -------------------- INITIALIZATION --------------------
    public string Instruction { get; protected set; }
    public List<string> Args { get; protected set; }
    
    public Command(string inst, List<string> args) {
        Instruction = inst;
        Args = args;
    }

    // -------------------- METHODS --------------------
    public virtual async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        await stderr.WriteLineAsync($"{Instruction}: command not found");
        return CmdResult.Ok;
    }
}