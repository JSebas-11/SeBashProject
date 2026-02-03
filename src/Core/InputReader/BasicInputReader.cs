using System.Text;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Core.Completion;
using SeBashProject.src.Core.History;

namespace SeBashProject.src.Core.InputReader;

internal class BasicInputReader : IInputReader {
    // --------------------- INIT ---------------------
    private readonly StringBuilder _buffer = new ();
    private string _tmpBuffer;
    private bool multMatches = false;
    private List<string> multMatchesList = [];
    private readonly HistoryNavigator _historyNavigator;

    public BasicInputReader(HistoryNavigator historyNavigator) 
        => _historyNavigator = historyNavigator;

    // --------------------- METHODS ---------------------
    public string? ReadLine() {
        Console.Write("$ ");
        ConsoleKeyInfo key;

        while (true) {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter) break;

            HandleKey(key);
        }

        Console.WriteLine();
        _tmpBuffer = _buffer.ToString();
        CleanChars(_buffer.Length);
        _buffer.Clear();
        return _tmpBuffer;
    }

    // --------------------- INTERNAL METHODS ---------------------
    private void Print(object text) => Console.Write(text.ToString());
    private void PrintColl(List<string> coll) {
        Console.WriteLine();
        foreach (string item in coll)
            Console.Write($"{item}  ");
        Console.WriteLine();
    }
    private void CleanChars(int count = 1) {
        for (int i = 0; i < count; i++) Console.Write("\b \b");
    }
    private void WriteLastBuffer() {
        Console.Write("$ ");
        Console.Write(_buffer.ToString());
    }

    private void HandleKey(ConsoleKeyInfo key) {
        if (key.Key != ConsoleKey.Tab) {
            multMatches = false;
            multMatchesList.Clear();
        }

        switch (key.Key) {
            
            case ConsoleKey.Backspace:
                if (_buffer.Length > 0) {
                    RemoveLastCharBuffer();
                    CleanChars();   
                }
                break;
            case ConsoleKey.Tab: 
                HandleCompletion();
                break;
            case ConsoleKey.UpArrow: 
            case ConsoleKey.DownArrow:
                HandleHistory(key.Key == ConsoleKey.DownArrow);
                break;
            case ConsoleKey.LeftArrow: break;
            case ConsoleKey.RightArrow: break;

            default:
                _historyNavigator.CancelNavigation();
                InsertInBuffer(key.KeyChar); 
                Print(key.KeyChar);
                break;
        };
    }

    private void HandleHistory(bool next) {
        if (!_historyNavigator.IsNavigating)
            _historyNavigator.BeginNavigation(_buffer.ToString());
            
        string? newBuffer = next 
            ? _historyNavigator.GetNext() : _historyNavigator.GetPrevious();
        
        if (newBuffer is null) return;

        CleanChars(_buffer.Length);
        UpdateBuffer(newBuffer);
        Print(_buffer);
    }

    private void HandleCompletion() {
        if (_buffer.Length == 0) return;

        if (multMatches) {
            PrintColl(multMatchesList);
            WriteLastBuffer();
            multMatches = false;
            return;
        }

        var compResult = CompletionEngine.Complete(_buffer.ToString());

        switch (compResult.CompletionType) {

            case CompletionType.NoMatch: 
                multMatches = false;
                Console.Write('\a');
                break;    

            case CompletionType.OneMatch:
                multMatches = false;
                CleanChars(_buffer.Length);
                UpdateBuffer($"{compResult.Matches[0]} ");
                Print(_buffer);
                break;

            case CompletionType.MultipleMatch:
                string? lcp = CompletionEngine.LongestCommonPrefix(compResult.Matches);
                if (!string.IsNullOrWhiteSpace(lcp)) {
                    CleanChars(_buffer.Length);
                    UpdateBuffer(lcp);
                    Print(_buffer);
                }

                Console.Write('\a');
                multMatches = true;
                multMatchesList = compResult.Matches;
                break;

            default: break;
        }
    }

    private void InsertInBuffer(char c) {
        if (c == '\0') return;

        _buffer.Append(c);
    }
    private void RemoveLastCharBuffer(){
        if (_buffer.Length < 1) return;

        _buffer.Length--;
    }
    private void UpdateBuffer(string newValue){
        _buffer.Clear();
        _buffer.Append(newValue);
    }
}