using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models.Execution;
using SeBashProject.src.Utilities;

namespace SeBashProject.src.Core.Execution;

internal class PipelineExecutor {
    // -------------------- INITIALIZATION --------------------
    public IReadOnlyList<ExecutionUnit> ExeUnits { get; protected set; }
    public int CommandsCount => ExeUnits.Count;
    public int PipesCount => CommandsCount-1;

    public PipelineExecutor(List<ExecutionUnit> exeUnits) => ExeUnits = exeUnits;

    // -------------------- METHODS --------------------
    public async Task<CmdResult> ExecuteAsync() {
        var readers = new System.IO.TextReader[CommandsCount];
        var writers = new TextWriter[CommandsCount];

        TextWriter? redStdout = null;
        TextWriter? redStderr = null;

        // Pipeline's begin - end
        readers[0] = Console.In;
        writers[CommandsCount-1] = Console.Out;
        
        try {
            // End Redirections
            var lastCtx = ExeUnits[CommandsCount-1].ContextExecution;
            if (lastCtx.Stdout is not null) {
                redStdout = StreamService.GenTextWriter(lastCtx.Stdout);
                writers[CommandsCount-1] = redStdout;
            }
            redStderr = lastCtx.Stderr is null 
                ? Console.Error : StreamService.GenTextWriter(lastCtx.Stderr);

            // Pipe Creation
            for (int i = 0; i < PipesCount; i++) {
                (var writer, var reader) = StreamService.CreateLinkedStreams();
                
                writers[i] = writer; 
                readers[i+1] = reader; 
            }

            // Execution
            var tasks = new List<Task>();
            for (int i = 0; i < CommandsCount; i++) {
                var cmd = ExeUnits[i].Command;

                // Last could have Stderr redirection (stdOut already handled)
                var stderr = i == CommandsCount-1 ? redStderr : Console.Error;

                tasks.Add(cmd.ExecuteAsync(readers[i], writers[i], stderr));
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex) { Console.WriteLine($"Pipeline error: {ex.Message}"); }
        finally {
            foreach (var r in readers) {
                if (r != null && r != Console.In)
                    r.Dispose();
            }

            foreach (var w in writers) {
                if (w != null && w != Console.Out)
                    w.Dispose();
            }

            if (redStdout != null && redStdout != Console.Out)
                redStdout.Dispose();

            if (redStderr != null && redStderr != Console.Error)
                redStderr.Dispose();

        }  
        return CmdResult.Ok;
    }
}