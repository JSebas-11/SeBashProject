using SeBashProject.src.Utilities.Os;

namespace SeBashProject.src.Core.History;

internal class HistoryService {
    // -------------------- INITIALIZATION --------------------
    private readonly List<string> _lines = [];
    private int _lastAppendedIndex = 0;
    public int TotalLines => _lines.Count;
    public string? ExternalFilePath { get; }

    public HistoryService(string? extFilePath) => ExternalFilePath = extFilePath;

    // -------------------- METHODS --------------------
    public void MarkLoadedAsAppended() => _lastAppendedIndex = _lines.Count;
    public void AddLine(string line) => _lines.Add(line);
    public IReadOnlyList<string> GetLines() => _lines;
    public IReadOnlyList<string> GetLastNLines(int n) {
        if (n > _lines.Count) n = _lines.Count;

        return _lines[^n..];
    }

    public string? GetLineIndex(int index)
        => index >= TotalLines || index < 0 ? null : _lines[index];

    public void AppendFromFiles(List<string> files) {
        foreach (var file in files) {
            if (!PathHandler.ExistsFile(file)) continue;
            
            try {
                foreach (var line in File.ReadLines(file)) {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    AddLine(line);
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }
    public void LinesToFiles(List<string> filePaths) {
        foreach (var filePath in filePaths) {
            try {
                using (var sw = new StreamWriter(filePath, false)) {
                    foreach (var line in _lines) 
                        sw.WriteLine(line);
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    public void AppendNewLinesToFiles(List<string> filePaths) {
        if (TotalLines <= _lastAppendedIndex) return;

        foreach (var filePath in filePaths) {
            using (var sw = new StreamWriter(filePath, true)) {
                for (int i = _lastAppendedIndex; i < TotalLines; i++)
                    sw.WriteLine(_lines[i]);
            }
        }

        _lastAppendedIndex = TotalLines;
    }

    public void LoadFromExtfile(){
        if (string.IsNullOrWhiteSpace(ExternalFilePath)) return;

        try {
            using (var sr = new StreamReader(ExternalFilePath)) {
                string? line;
                while ((line = sr.ReadLine()) != null) _lines.Add(line);
            }
        }
        catch (FileNotFoundException) { }            
        catch (IOException) { }            
    }

    public void SaveToExtfile() {
        if (string.IsNullOrWhiteSpace(ExternalFilePath)) return;

        AppendNewLinesToFiles( [ExternalFilePath] );         
    }
}