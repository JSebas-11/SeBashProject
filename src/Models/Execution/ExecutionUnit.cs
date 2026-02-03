namespace SeBashProject.src.Models.Execution;

internal sealed class ExecutionUnit {
    public Command Command { get; }
    public ContextExecution ContextExecution { get; }

    public ExecutionUnit(Command comm, ContextExecution ctxExe) {
        Command = comm;
        ContextExecution = ctxExe;
    }
}