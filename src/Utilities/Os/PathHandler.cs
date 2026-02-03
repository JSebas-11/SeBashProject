namespace SeBashProject.src.Utilities.Os;

public static class PathHandler {
    public static string ConcatEnvHome(string path)
        => $"{OsInteraction.GetEnvironmentHome()}{path[1..]}";
    public static bool ExistsDirectory(string path)
        => Directory.Exists(path);
    public static bool ExistsFile(string path)
        => File.Exists(path);
}