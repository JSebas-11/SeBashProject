using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeBashProject.src;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Drawing;
using SeBashProject.src.Core.Execution;
using SeBashProject.src.Core.History;
using SeBashProject.src.Core.InputReader;
using SeBashProject.src.Core.Interpretation;
using SeBashProject.src.Core.Parsing;
using SeBashProject.src.Utilities;

class Program {
    static void Main() {
        var collection = new ServiceCollection();
        
        // CONFIGURATION
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        // DEPENDENCIES
        collection.AddSeBashDependencies(config);

        ServiceProvider provider = collection.BuildServiceProvider();
        // EXECUTION SERVICES
        HistoryService historyService = provider.GetRequiredService<HistoryService>();
        CommandFactory cmdFactory = provider.GetRequiredService<CommandFactory>();
        IInputReader reader = provider.GetRequiredService<IInputReader>();

        string? input;

        historyService.LoadFromExtfile();

        while (true) {
            TerminalWriter.WriteHeader();
            input = reader.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;

            // HISTORYEXPANSION
            historyService.AddLine(input);
            var tokens = Parser.GetTokens(input);
            var cExe = Interpreter.GenExecutorContexts(tokens);
            var exeUnits = cmdFactory.CreateExecutionUnits(cExe);

            var cmdResult = Executor.ExecuteAsync(exeUnits).GetAwaiter().GetResult();

            if (cmdResult == CmdResult.Exit) {
                historyService.SaveToExtfile();
                break;
            };
        }
    }
}