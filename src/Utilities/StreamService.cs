using System.IO.Pipes;
using SeBashProject.src.Common.Enums;
using SeBashProject.src.Models.Execution;

namespace SeBashProject.src.Utilities;

internal static class StreamService {
    // ----------------------- METHODS -----------------------
    public static async Task PipeWrittingAsync(
        TextReader from, TextWriter to, Action? onCompleted = null
    ){
        char[] buffer = new char[4096];
        int read;

        while ((read = await from.ReadAsync(buffer, 0, buffer.Length)) > 0) {
            await to.WriteAsync(buffer, 0, read);
            await to.FlushAsync();
        }

        onCompleted?.Invoke();
    }

    public static (TextWriter writer, TextReader reader) CreateLinkedStreams(){
        var pipeOut = new AnonymousPipeServerStream(PipeDirection.Out);
        var pipeIn = new AnonymousPipeClientStream(PipeDirection.In, pipeOut.ClientSafePipeHandle);

        return (
            new StreamWriter(pipeOut) { AutoFlush = true },
            new StreamReader(pipeIn)
        );
    }

    public static TextWriter GenTextWriter(Redirection red) 
        => new StreamWriter(GenFileStream(red)) { AutoFlush = true };

    // ----------------------- INTERNAL METHODS -----------------------
    private static FileStream GenFileStream(Redirection red) {
        FileMode fMode = red.RedirectionType == RedirectionType.Append 
            ? FileMode.Append : FileMode.Create;

        return new FileStream(red.Target, fMode, FileAccess.Write);
    }
}