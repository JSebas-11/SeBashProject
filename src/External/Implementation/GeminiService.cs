using SeBashProject.src.External.Abstraction;
using SeBashProject.src.External.Config;

namespace SeBashProject.src.External.Implementation;

internal class GeminiService : IGenerativeService {
    // --------------------- INIT ---------------------
    private readonly HttpClient _httpClient;
    private readonly GenServiceConfig _config;
    private readonly string _endPoint;

    public GeminiService(HttpClient httpClient, GenServiceConfig config) {
        _httpClient = httpClient;
        _config = config;
        _endPoint = $"https://generativelanguage.googleapis.com/v1beta/models/{_config.Model}:generateContent?key={_config.ApiKey}";
    }

    // --------------------- METHODS ---------------------
    public async Task<string> ExplainCommandAsync(string command) {
        return "Explain Not Implemented";
    }

    public async Task<string> ExplainFileAsync(string content) {
        return "Explain-File Not Implemented";
    }

    public async Task<string> GenerateCommandAsync(string prompt) {
        return "Generate Not Implemented";
    }

    public async Task<string> HistoryBasedAsync(IEnumerable<string> history, string prompt) {
        return "History-based Not Implemented";
    }

    public async Task<string> SummarizeAsync(string content) {
        return "Summarize Not Implemented";
    }
}