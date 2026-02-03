using SeBashProject.src.Common.Enums;

namespace SeBashProject.src.Models;

internal sealed class Token {
    public TokenType TokenType { get; private set; }
    public string Value { get; private set; } = null!;
    public int? FileDescriptor { get; } // 1 = stdout, 2 = stderr

    public Token(TokenType type, string value, int? fileDesc = null) {
        TokenType = type;
        Value = value;
        FileDescriptor = fileDesc;
    }
}