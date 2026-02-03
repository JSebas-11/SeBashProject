namespace SeBashProject.src.Common.Enums;

internal enum TokenType { 
    Word, Redirect, RedirectAppend, Pipeline
}

internal enum RedirectionType { Truncate, Append }

internal enum CmdResult { Ok, Exit }

internal enum CompletionType { NoMatch, OneMatch, MultipleMatch }