namespace SeBashProject.src.Core.History;

internal class HistoryConfig {
    public int MaxHistory { get; set; }
    public bool SaveDuplicates { get; set; }
    public string? FilePath { get; set; }
    public string[] Excludes { get; set; } = [];
}