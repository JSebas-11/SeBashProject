using System.Diagnostics;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models;
using SeBashProject.src.Utilities;

namespace SeBashProject.src.Commands;

internal class ExternalCommand : Command {
    public string ProgramName { get; private set; }
    public string ExePath { get; private set; }

    public ExternalCommand(string name, string exePath, List<string> args) 
        : base(name, args) {
        ProgramName = name;
        ExePath = exePath;
    }

    public override async Task<CmdResult> ExecuteAsync(
        TextReader stdin, TextWriter stdout, TextWriter stderr
    ) {
        var psi = new ProcessStartInfo(ProgramName) { 
            UseShellExecute = false, 
            RedirectStandardInput = stdin != Console.In, 
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        foreach (var arg in Args) psi.ArgumentList.Add(arg);

        try {
            using var process = Process.Start(psi)!;

            List<Task> pipesWritting = [
                StreamService.PipeWrittingAsync(
                    process.StandardOutput, stdout, () => stdout.Close()
                ),
                StreamService.PipeWrittingAsync(process.StandardError, stderr),
            ];
            if (stdin != Console.In) // Not redirect when execution is simple (not pipeline)
                pipesWritting.Add(
                    StreamService.PipeWrittingAsync(
                        stdin, process.StandardInput, () => process.StandardInput.Close()
                    )
                );

            await process.WaitForExitAsync();
            await Task.WhenAll(pipesWritting);
        }
        catch (Exception) { }
        return CmdResult.Ok;
    }
}