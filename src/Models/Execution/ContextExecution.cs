namespace SeBashProject.src.Models.Execution;

internal sealed class ContextExecution {
    public string Instruction { get; }
    public List<string> Args { get; }
    public Redirection? Stdout { get; }
    public Redirection? Stderr { get; }
    
    public ContextExecution(string inst, List<string> args, 
        Redirection? stdout, Redirection? stderr) 
    {
        Instruction = inst;
        Args = args;
        Stdout = stdout;
        Stderr = stderr;
    }
}