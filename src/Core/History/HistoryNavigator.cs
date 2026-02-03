namespace SeBashProject.src.Core.History;

internal class HistoryNavigator {
    // --------------------- INIT ---------------------
    private int currentIndex;
    private string? _draftBuffer;
    private readonly HistoryService _historyService;

    public bool IsNavigating { get; private set; }
    public HistoryNavigator(HistoryService historyService) {
        _historyService = historyService;
        currentIndex = _historyService.TotalLines;
    }

    // --------------------- METHODS ---------------------
    public void BeginNavigation(string currentBuffer) {
        if (IsNavigating) return;
        
        IsNavigating = true;
        _draftBuffer = currentBuffer;
        currentIndex = _historyService.TotalLines;
    } 
    public void CancelNavigation() {
        IsNavigating = false;
        _draftBuffer = null;
        currentIndex = _historyService.TotalLines;
    } 
    public string? GetPrevious() {
        if (_historyService.TotalLines == 0) return null;
        if (currentIndex > 0) currentIndex--;
        
        return _historyService.GetLineIndex(currentIndex);
    }
    public string? GetNext() {
        int total = _historyService.TotalLines;
        if (currentIndex < total) currentIndex++;
        if (currentIndex == total) return _draftBuffer;

        return _historyService.GetLineIndex(currentIndex);
    }
}