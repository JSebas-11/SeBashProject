using SeBashProject.src.Common.Enums;

namespace SeBashProject.src.Models.Execution;

internal sealed class Redirection {
    public RedirectionType RedirectionType { get; }
    public string Target { get; }

    public Redirection(RedirectionType redType, string target) {
        RedirectionType = redType;
        Target = target;
    }
}