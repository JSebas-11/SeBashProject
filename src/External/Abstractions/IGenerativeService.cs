namespace SeBashProject.src.External.Abstraction;

internal interface IGenerativeService {
    Task<string> ExplainCommandAsync(string command);
    Task<string> ExplainFileAsync(string content);
    Task<string> GenerateCommandAsync(string prompt);
    Task<string> SummarizeAsync(string content);
    Task<string> HistoryBasedAsync(IEnumerable<string> history, string prompt);
}