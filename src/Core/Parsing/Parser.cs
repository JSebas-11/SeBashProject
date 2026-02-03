using System.Text;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models;

namespace SeBashProject.src.Core.Parsing;

internal static class Parser {
    private static readonly List<char> _escapeInDouble = ['"', '\\'];
    
    public static List<Token> GetTokens(string input) {
        List<Token> args = [];
        
        input = input.Trim();
        // STATES
        bool InSingle = false,
            BackslashEsc = false,
            InDouble = false;

        var current = new StringBuilder();
        for (int i = 0; i < input.Length; i++) { 
            char c = input[i];

            // Activate backslash Escaping
            if (c == '\\' && !BackslashEsc && !InSingle) {
                BackslashEsc = true;
                continue;
            }

            if (BackslashEsc) {
                // SPECIAL WORD ESCAPING INTO QUOTES
                BackslashEsc = false;

                if (InDouble) {
                    if (!_escapeInDouble.Any(ch => ch == c))
                        current.Append('\\');
                    
                    current.Append(c);
                    
                } else { current.Append(c); }

                continue;
            }

            // QUOTES ACTIVATE/DEACTIVATE
            if (c == '\'' && !InDouble) {
                InSingle = !InSingle;
                continue;
            }
            if (c == '\"' && !InSingle) {
                InDouble = !InDouble;
                continue;
            }
            
            // HANDLING REDIRECTION
            if (c == '>' && !InSingle && !InDouble) {
                int fd = 1;

                if (current.Length > 0) {
                    if (int.TryParse(current.ToString(), out int parsedFd))
                        fd = parsedFd;
                    else
                        args.Add(new Token(TokenType.Word, current.ToString()));
                    
                    current.Clear();
                }

                bool isAppend = i+1 < input.Length && input[1+i] == '>';
                var token = isAppend ? new Token(TokenType.RedirectAppend, ">>", fd) 
                    : new Token(TokenType.Redirect, ">", fd);
                
                if (isAppend) i++;

                args.Add(token);
                continue;
            }

            // HANDLING PIPELINES
            if (c == '|' && !InSingle && !InDouble) {
                if (current.Length > 0) {
                    args.Add(new Token(TokenType.Word, current.ToString()));
                    current.Clear();
                }

                args.Add(new Token(TokenType.Pipeline, ""));
                continue;
            }

            // WHITESPACE
            if (char.IsWhiteSpace(c) && !InSingle && !InDouble) {
                if (current.Length > 0) {
                    args.Add(new Token(TokenType.Word, current.ToString()));
                    current.Clear();
                }

                continue;
            }

            current.Append(c);
        }

        if (current.Length > 0) args.Add(new Token(TokenType.Word, current.ToString()));

        return args;
    }
}