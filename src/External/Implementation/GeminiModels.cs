using System.Text.Json.Serialization;

namespace SeBashProject.src.External.Implementation;

// --------------------- COMMON ---------------------
internal class Content(Part[] parts, string role = "model") {
    [JsonPropertyName("parts")]
    public Part[] Parts { get; init; } = parts;
    [JsonPropertyName("role")]
    public string Role { get; init; } = role;
}

internal class Part(string text) {
    [JsonPropertyName("text")]
    public string Text { get; init; } = text;
}

// --------------------- REQUETS ---------------------
internal class GeminiRequest(Content[] contents) {
    [JsonPropertyName("contents")]
    public Content[] Contents { get; init; } = contents;
}

// --------------------- RESPONSE ---------------------
internal class GeminiResponse(Candidate[] candidates) {
    [JsonPropertyName("candidates")]
    public Candidate[] Candidates { get; init; } = candidates;

    public string? GetFirstResponse()
        => Candidates?.FirstOrDefault()?.Content?.Parts?.FirstOrDefault()?.Text;
}

internal class Candidate(Content content) {
    [JsonPropertyName("content")]
    public Content Content { get; init; } = content;
}