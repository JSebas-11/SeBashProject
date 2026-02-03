namespace SeBashProject.src.Common;

internal static class ShellMetadata {
    public static List<string> Builtins { get; } = [
        "echo", "exit", "type", "pwd", "cd", "history"
    ];

    public static bool IsBuiltin(string command)
        => Builtins.Any(e => e == command);
}