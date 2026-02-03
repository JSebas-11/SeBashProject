using System.Text;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Completion;
using SeBashProject.src.Core.History;
using SeBashProject.src.Models;

namespace SeBashProject.src.Core.InputReader;

internal class InteractiveInputReader : IInputReader {
    // --------------------- INIT ---------------------
    private readonly StringBuilder _buffer = new ();
    private int prevRenderLen;
    private int startLeft;
    private int startTop;
    private bool multMatchesAvailable = false;
    private bool shouldRenderMatches = false;
    private List<string> multMatchesList = [];
    private readonly HistoryNavigator _historyNavigator;

    public InteractiveInputReader(HistoryNavigator historyNavigator)
        => _historyNavigator = historyNavigator;

    // --------------------- METHODS ---------------------
    public string? ReadLine() {
        ConsoleKeyInfo key;
        prevRenderLen = 0;

        Console.Write("$ ");
        (startLeft, startTop) = Console.GetCursorPosition();
        
        do {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Enter) {
                Console.WriteLine();
                break;
            }
            
            HandleKey(key);
            Render();
            
        } while (key.Key != ConsoleKey.Enter);
        
        var buffer = _buffer.ToString();
        _buffer.Clear();
        return buffer;
    }

    // --------------------- INTERNAL METHODS ---------------------
    // RENDER
    private void Render(){
        if (shouldRenderMatches) {
            RenderColl(multMatchesList);
            shouldRenderMatches = false;
        }

        Console.SetCursorPosition(startLeft, startTop);
        if (prevRenderLen > 0)
            Console.Write(new string(' ', prevRenderLen));
        
        Console.SetCursorPosition(startLeft, startTop);
        Console.Write(_buffer);

        prevRenderLen = _buffer.Length;
        Console.SetCursorPosition(startLeft + _buffer.Length, startTop);
    }
    private void RenderColl(List<string> coll) {
        Console.WriteLine();
            foreach (var item in coll)
                Console.Write($"{item}  ");
        Console.WriteLine();

        Console.Write("$ ");
        (startLeft, startTop) = Console.GetCursorPosition();
        prevRenderLen = 0;
    }
    
    // BUFFER
    private void InsertInBuffer(char c) {
        if (c == '\0') return;

        _buffer.Append(c);
    }
        
    private void RemoveLastCharBuffer(){
        if (_buffer.Length < 1) return;

        _buffer.Length--;
    }
    private void ReplaceLastInBuffer(string newWord) {
        int lastIndex = LastTokenIndex();

        _buffer.Remove(lastIndex, _buffer.Length-lastIndex);
        _buffer.Append(newWord);
    }
    private void ReplaceBuffer(string newBuffer) {
        _buffer.Clear();
        _buffer.Append(newBuffer);
    }
    private int LastTokenIndex(){
        for (int i = _buffer.Length-1; i >= 0; i--) {
            if (_buffer[i] == ' ') return i+1;
        }

        return 0;
    }
    private string GetLastToken(){
        var lastToken = new StringBuilder();

        for (int i = LastTokenIndex(); i < _buffer.Length; i++)
            lastToken.Append(_buffer[i]);

        return lastToken.ToString();
    }

    // KEYS
    private void HandleKey(ConsoleKeyInfo key) {
        if (key.Key != ConsoleKey.Tab) {
            multMatchesAvailable = false;
            multMatchesList.Clear();
        }

        switch (key.Key) {

            case ConsoleKey.Backspace: RemoveLastCharBuffer(); break;
            case ConsoleKey.Tab: HandleTab(); break;
            case ConsoleKey.UpArrow: case ConsoleKey.DownArrow: 
                HandleHistory(key.Key == ConsoleKey.DownArrow);
                break;
            case ConsoleKey.LeftArrow: break;
            case ConsoleKey.RightArrow: break;

            default: 
                _historyNavigator.CancelNavigation();
                InsertInBuffer(key.KeyChar); 
                break;
        }
    }
    private void HandleTab(){
        if (multMatchesAvailable) {
            shouldRenderMatches = true;
            return;
        }

        string toComplete = GetLastToken();
        if (string.IsNullOrWhiteSpace(toComplete)) return;

        CompletionResult result = CompletionEngine.Complete(toComplete);
        
        switch (result.CompletionType) {

            case CompletionType.NoMatch: 
                multMatchesAvailable = false;
                Console.Write('\a');
                break;
            case CompletionType.OneMatch: 
                multMatchesAvailable = false;
                ReplaceLastInBuffer($"{result.Matches[0]} "); 
                break;
            case CompletionType.MultipleMatch:
                multMatchesList = result.Matches;

                string? lcp = CompletionEngine.LongestCommonPrefix(multMatchesList);
                if (!string.IsNullOrWhiteSpace(lcp)) 
                    ReplaceLastInBuffer(lcp);

                Console.Write('\a');
                multMatchesAvailable = true;
                break;
            default: break;
        }
    }
    private void HandleHistory(bool next) {
        if (!_historyNavigator.IsNavigating) 
            _historyNavigator.BeginNavigation(_buffer.ToString());

        string? newBuffer = next 
            ? _historyNavigator.GetNext() : _historyNavigator.GetPrevious();

        if (string.IsNullOrWhiteSpace(newBuffer)) return;

        ReplaceBuffer(newBuffer);
    }
}