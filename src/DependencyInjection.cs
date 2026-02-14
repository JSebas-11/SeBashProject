using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SeBashProject.src.Core.History;
using SeBashProject.src.Core.InputReader;
using SeBashProject.src.External.Abstraction;
using SeBashProject.src.External.Config;
using SeBashProject.src.External.Implementation;
using SeBashProject.src.Utilities;
using SeBashProject.src.Utilities.Os;
using Serilog;

namespace SeBashProject.src;

internal static class DependencyInjection {
    public static IServiceCollection AddSeBashDependencies(
        this IServiceCollection services, IConfiguration config
    ) {
        //CONFIGURATION
        services.AddSingleton(config);

        var historyConf = config.GetSection("HistoryConfig").Get<HistoryConfig>()!;
        // Add full path of history file
        historyConf.FilePath = string.IsNullOrWhiteSpace(historyConf.FilePath) 
            ? null : PathHandler.Concat2AppBaseDirectory(historyConf.FilePath);

        // LOGGING
        var logPath = config.GetValue<string>("LogFilePath") 
            ?? throw new InvalidOperationException("Logging File not configured");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(PathHandler.Concat2AppBaseDirectory(logPath))
            .CreateLogger();

        services.AddLogging(builder => {
            builder.ClearProviders();
            builder.AddSerilog();
        });

        // FACTORY
        services.AddSingleton<CommandFactory>();

        // HISTORY
        services.AddSingleton(historyConf);
        services.AddSingleton<HistoryService>();
        services.AddSingleton<HistoryNavigator>();

        // TEXTREADER
        services.AddSingleton<IInputReader, InteractiveInputReader>();
        
        // AI
        var genServiceConf = config.GetSection("GenerativeService").Get<GenServiceConfig>() 
            ?? throw new InvalidOperationException("GenerativeService not configured");

        services.AddSingleton(genServiceConf);
        services.AddHttpClient<IGenerativeService, GeminiService>();

        return services;
    }
}