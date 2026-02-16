using SeBashProject.src.Common.Enums;
using SeBashProject.src.Utilities.Os;
using Spectre.Console;

namespace SeBashProject.src.Core.Drawing;

internal static class TerminalWriter {
    // --------------------- METHODS ---------------------
    public static void WriteHeader() {
        string user = Environment.UserName;
        string machine = Environment.MachineName;
        string cwd = OsInteraction.GetCurrentDirectory();

        AnsiConsole.MarkupLine(
            $"[bold green] {user}[/][bold white]" + 
            $"@[/][bold green]{machine}[/][white]:[/] [bold green]{cwd}[/]"
        );
    }

    public static async Task WriteLineAsync(
        string content, TextWriter writer, 
        TerminalStyles style = TerminalStyles.Default, string panelHeader = "Info"
    ) {
        if (ReferenceEquals(writer, Console.Out) || ReferenceEquals(writer, Console.Error))
            Render(content, style, panelHeader); 
        else 
            await writer.WriteLineAsync(content);
    }
    
    // --------------------- PRIVATE METHODS ---------------------
    private static void Render(string content, TerminalStyles style, string panelHeader) {
        switch (style) {
            case TerminalStyles.Error: 
                AnsiConsole.MarkupLine($"[bold red]{content}[/]");
                break;

            case TerminalStyles.Panel:
                var panel = new Panel(new Text(content))
                    .Header(panelHeader)
                    .Border(BoxBorder.Rounded)
                    .Padding(new (1));
                AnsiConsole.Write(panel);
                break;

            case TerminalStyles.Default:
            default:
                AnsiConsole.MarkupLine($"[silver]{content}[/]");
                break;
        }
    }
}