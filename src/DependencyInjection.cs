using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SeBashProject.src.Core.History;
using SeBashProject.src.Core.InputReader;
using SeBashProject.src.External.Abstraction;
using SeBashProject.src.External.Config;
using SeBashProject.src.External.Implementation;
using SeBashProject.src.Utilities;

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
            ? null : Path.Combine(AppContext.BaseDirectory, historyConf.FilePath);

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