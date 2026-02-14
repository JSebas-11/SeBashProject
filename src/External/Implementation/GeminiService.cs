using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using SeBashProject.src.External.Abstraction;
using SeBashProject.src.External.Config;

namespace SeBashProject.src.External.Implementation;

internal class GeminiService : IGenerativeService {
    // --------------------- INIT ---------------------
    private readonly HttpClient _httpClient;
    private readonly GenServiceConfig _config;
    private readonly ILogger<GeminiService> _logger;
    private readonly string _endPoint;

    public GeminiService(HttpClient httpClient, GenServiceConfig config, ILogger<GeminiService> logger) {
        _httpClient = httpClient;
        _config = config;
        _logger = logger;
        _endPoint = $"https://generativelanguage.googleapis.com/v1beta/models/{_config.Model}:generateContent?key={_config.ApiKey}";
    }

    // --------------------- METHODS ---------------------
    public Task<string> ExplainCommandAsync(string command)
        => LookUpGeminiAsync($@"
        Explain the following Linux command clearly and accurately:

        COMMAND:
        {command}

        GUIDELINES:
        - Break it down by parts (flags, arguments, subcommands).
        - Explain what each part does.
        - Describe what the full command accomplishes.
        - If the command is dangerous, say so and explain why.
        - If the command is ambiguous, list possible interpretations.", "ExplainCommandAsync");

    public Task<string> ExplainFileAsync(string content)
        => LookUpGeminiAsync($@"
        Explain the following file or code content:

        CONTENT:
        {content}

        GUIDELINES:
        - Summarize its purpose.
        - Explain how it works section by section.
        - Identify potential problems, errors, or security issues.
        - Describe any dependencies or assumptions.", "ExplainFileAsync");

    public Task<string> GenerateCommandAsync(string prompt)
        => LookUpGeminiAsync($@"
        Based on the following request, generate ONE safe Linux shell command:

        REQUEST:
        {prompt}

        RULES:
        - The command must be non-destructive (no rm -rf, wipefs, mkfs, dd to disks, etc.).
        - Prefer read-only or harmless commands unless explicitly stated.", "GenerateCommandAsync");

    public Task<string> SummarizeAsync(string content)
        => LookUpGeminiAsync($@"
        Provide a clear and concise summary of the following text:
        
        TEXT:
        {content}
        
        GUIDELINES:
        - Highlight key points.
        - Point out any errors, contradictions, or unclear areas.
        - Identify important causes or consequences mentioned in the text.
        - Make the summary neutral and objective.", "SummarizeAsync");
    
    public Task<string> HistoryBasedAsync(IEnumerable<string> history, string prompt) 
        => LookUpGeminiAsync($@"
        Based on the following Bash history and the new request, generate a helpful suggestion:

        REQUEST:
        {prompt}

        BASH HISTORY:
        {string.Join("\n", history)}

        GUIDELINES:
        - Identify patterns or repeated commands.
        - Suggest a useful next command or improvement.
        - Explain why the suggestion is relevant.
        - Do NOT generate dangerous commands (no rm -rf, dd, mkfs, etc.).", "HistoryBasedAsync");

    // --------------------- PRIVATE METHODS ---------------------
    private async Task<string> LookUpGeminiAsync(string prompt, string methodName) {
        // Simple request creation based on the prompt
        var request = new GeminiRequest(
            [ new Content( [ new Part(prompt) ] ) ]
        );
        
        try {
            var response = await _httpClient.PostAsJsonAsync(_endPoint, request);

            if (!response.IsSuccessStatusCode) {
                _logger.LogWarning(
                    "GeminiApi returned HTTP {Status} for method '{MethodName}'", 
                    response.StatusCode, methodName
                );
                return "GenerativeService returned an unexpected response";
            }
            
            var gemResponse = await response.Content.ReadFromJsonAsync<GeminiResponse>();
            var output = gemResponse?.GetFirstResponse();

            if (output is null) {
                _logger.LogWarning("GeminiApi: empty or malformed response for method '{MethodName}'", methodName);
                return "Error with GenerativeService";
            }

            return output;
        }
        catch (HttpRequestException httpEx) {
            _logger.LogError(httpEx, "GeminiApi HTTP error when executing method '{MethodName}'", methodName);
            return "Response was not reached";
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Unexpected error in method '{MethodName}'", methodName);
            return "There has been an error with GenerativeService";
        }
    }
}