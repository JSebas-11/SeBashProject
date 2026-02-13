namespace SeBashProject.src.Utilities.Os;

public static class OsInteraction {
    public static string? GetHistFilePath()
        => Environment.GetEnvironmentVariable("HISTFILE");
    public static string GetEnvironmentHome()
        => Environment.GetEnvironmentVariable("HOME");

    public static string GetCurrentDirectory()
        => Directory.GetCurrentDirectory();

    public static void ChangeCurrentDirectory(string newDir)
        => Directory.SetCurrentDirectory(newDir);

    public static IEnumerable<string> GetPathDirs() {
        string? pathVars = Environment.GetEnvironmentVariable("PATH");

        return pathVars is null ? [] : [.. pathVars.Split(":")];
    }

    public static IEnumerable<string> GetPathFiles() {
        List<string> files = [];

        foreach (string dir in GetPathDirs()) {
            try {
                foreach (string file in Directory.GetFiles(dir)) {
                    if (IsExecutable(file)) files.Add(file);
                }
            }
            catch (Exception) { continue; }
        }

        return files;
    }

    public static string? ExecutablePath(string input) {
        foreach (var pathDir in GetPathDirs()) {
            string commandPath = Path.Combine(pathDir, input);

            if (File.Exists(commandPath)) {
                if (IsExecutable(commandPath))
                    return commandPath;
            }
        }

        return null;
    }

    public static bool IsExecutable(string path) {
        try {
            UnixFileMode uFileMode = File.GetUnixFileMode(path);

            if (uFileMode.HasFlag(UnixFileMode.UserExecute) ||
                uFileMode.HasFlag(UnixFileMode.GroupExecute) ||
                uFileMode.HasFlag(UnixFileMode.OtherExecute)
            )
                return true;

            return false;
        }
        catch (Exception) { return false; }
    }
    
    public static async Task<string?> ContentFileAsync(string path) {
        if (!PathHandler.ExistsFile(path)) return null;

        try { return await File.ReadAllTextAsync(path); }
        catch (IOException) { return null; }
    }
}