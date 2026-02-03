using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models.Execution;

namespace SeBashProject.src.Builders;

internal class ContextExecutionBuilder {
    private string _instruction = "";
    private readonly List<string> _args = [];
    private Redirection? _stdout = null;
    private Redirection? _stderr = null;

    public ContextExecutionBuilder(){}
    
    // BUILD METHODS
    public ContextExecution Build() 
        => new ContextExecution(_instruction, [.. _args], _stdout, _stderr);
    public bool IsValid() => !string.IsNullOrWhiteSpace(_instruction);
    public void Clear() {
        _instruction = "";
        _args.Clear();
        _stdout = null;
        _stderr = null;
    }

    // BUILD METHODS
    public void WithInstruction(string inst) => _instruction = inst;
    public void AddArg(string arg) => _args.Add(arg);
    public void WithStdout(RedirectionType type, string target) 
        => _stdout = new Redirection(type, target);
    public void WithStderr(RedirectionType type, string target)
        => _stderr = new Redirection(type, target);    
}