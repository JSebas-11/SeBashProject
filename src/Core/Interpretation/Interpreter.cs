using SeBashProject.src.Builders;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models;
using SeBashProject.src.Models.Execution;

namespace SeBashProject.src.Core.Interpretation;

internal static class Interpreter {
    public static List<ContextExecution> GenExecutorContexts(List<Token> tokens) {
        List<ContextExecution> contextExes = [];
        var context = new ContextExecutionBuilder();

        bool isInstruction = true;

        for (int i = 0; i < tokens.Count; i++) {
            Token token = tokens[i];

            // INSTRUCTION INTERPRETATION
            if (isInstruction && token.TokenType == TokenType.Word) {
                context.WithInstruction(token.Value);
                isInstruction = false;
                continue;
            }

            // PIPELINE INTERPRETATION
            if (token.TokenType == TokenType.Pipeline) {
                if (context.IsValid()) contextExes.Add(context.Build());
                context.Clear();
                isInstruction = true;
                continue;
            }

            // REDIRECTIONS INTERPRETATION
            if (token.TokenType == TokenType.Redirect || token.TokenType == TokenType.RedirectAppend) {
                i++;

                if (i >= tokens.Count || tokens[i].TokenType != TokenType.Word) continue;

                string redTarget = tokens[i].Value;
                var redType = token.TokenType == TokenType.Redirect
                    ? RedirectionType.Truncate : RedirectionType.Append;

                if (token.FileDescriptor == 1)
                    context.WithStdout(redType, redTarget);
                else if (token.FileDescriptor == 2)
                    context.WithStderr(redType, redTarget);

                continue;
            }

            // ARGUMENT INTERPRETATION
            context.AddArg(token.Value);
        }

        if (context.IsValid()) contextExes.Add(context.Build());

        return contextExes;
    }
}