using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models.Execution;
using SeBashProject.src.Utilities;

namespace SeBashProject.src.Core.Execution;

internal static class Executor {
    public static async Task<CmdResult> ExecuteAsync(List<ExecutionUnit> exeUnits) {
        if (exeUnits.Count == 1) {
            return await SimpleExecutionAsync(exeUnits[0]);
        }

        return await new PipelineExecutor(exeUnits).ExecuteAsync();
    }

    private static async Task<CmdResult> SimpleExecutionAsync(ExecutionUnit exeUnit) {
        var context = exeUnit.ContextExecution;
        TextWriter? stdoutWriter = null;
        TextWriter? stderrWriter = null;

        try {
            var stdout = Console.Out;
            var stderr = Console.Error;

            if (context.Stdout is not null) {
                stdoutWriter = StreamService.GenTextWriter(context.Stdout);
                stdout = stdoutWriter;
            }

            if (context.Stderr is not null) {
                stderrWriter = StreamService.GenTextWriter(context.Stderr);
                stderr = stderrWriter;
            }

            return await exeUnit.Command.ExecuteAsync(Console.In, stdout, stderr);
        }
        catch (Exception) { return CmdResult.Ok; }
        finally {
            stdoutWriter?.Close();
            stderrWriter?.Close();
        }
    }
}