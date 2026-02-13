using SeBashProject.src.Commands;
using SeBashProject.src.Commands.Builtin;
using SeBashProject.src.Common;
using SeBashProject.src.Core.History;
using SeBashProject.src.External.Abstraction;
using SeBashProject.src.Models;
using SeBashProject.src.Models.Execution;
using SeBashProject.src.Utilities.Os;

namespace SeBashProject.src.Utilities;

internal class CommandFactory {
    // ----------------------- INIT -----------------------
    private readonly HistoryService _historyService;
    private readonly IGenerativeService _genService;

    public CommandFactory(HistoryService historyService, IGenerativeService generativeService) {
        _historyService = historyService;
        _genService = generativeService;
    }


    // ----------------------- METHODS -----------------------
    public ExecutionUnit CreateExeUnit(ContextExecution context) {
        Command command = CreateCommand(context);
        return new ExecutionUnit(command, context);
    }

    public Command CreateCommand(ContextExecution context) {
        string instruction = context.Instruction;
        List<string> args = context.Args;
        
        if (ShellMetadata.IsBuiltin(instruction)) {
            return instruction switch {
                "exit" => new ExitCommand(),
                "echo" => new EchoCommand(args),
                "type" => new TypeCommand(args),
                "cd" => new CdCommand(args),
                "pwd" => new PwdCommand(),
                "history" => new HistoryCommand(_historyService, args),
                "tian" => new TianCommand(_genService, _historyService, args),
                _ => new Command(instruction, args)
            };
        }

        string? exePath = OsInteraction.ExecutablePath(instruction);

        return exePath is null ? new Command(instruction, args)
            : new ExternalCommand(instruction, exePath, args);
    }

    public List<ExecutionUnit> CreateExecutionUnits(List<ContextExecution> contexts) {
        List<ExecutionUnit> commandsCtx = [];

        foreach (var context in contexts)
            commandsCtx.Add(CreateExeUnit(context));

        return commandsCtx;
    }
}