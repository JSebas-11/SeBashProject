using SeBashProject.src.Common.Enums;

namespace SeBashProject.src.Core.Execution;

internal interface IExecutable {
    Task<CmdResult> ExecuteAsync(TextReader stdin, TextWriter stdout, TextWriter stderr);
}