using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Drawing;
using SeBashProject.src.Models;
using SeBashProject.src.Utilities.Os;

namespace SeBashProject.src.Commands.Builtin;

internal sealed class CdCommand : Command {
    public CdCommand(List<string> arg) : base("cd", arg) { }

    public override async Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr) {
        if (Args.Count > 1) {
            await TerminalWriter.WriteLineAsync("cd: too many arguments", stderr, TerminalStyles.Error);
            return CmdResult.Ok;
        }

        string orContent = Args.Count == 0 ? OsInteraction.GetEnvironmentHome() : Args[0];
        string path = orContent.StartsWith('~') ? PathHandler.ConcatEnvHome(orContent) 
            : orContent;

        if (!PathHandler.ExistsDirectory(path)) {
            await TerminalWriter.WriteLineAsync(
                $"cd: {orContent}: No such file or directory", stderr, TerminalStyles.Error
            );
            return CmdResult.Ok;
        }

        OsInteraction.ChangeCurrentDirectory(path);

        return CmdResult.Ok;
    }
}