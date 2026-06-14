using System.Text;

namespace MensaApp;

public static class CrashLogger
{
    private static readonly object LockObject = new();

    public static string LogFilePath
    {
        get
        {
            var folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MensaApp");

            Directory.CreateDirectory(folder);

            return Path.Combine(folder, "crashlog.txt");
        }
    }

    public static void Log(string source, Exception? exception = null, string? extra = null)
    {
        try
        {
            var sb = new StringBuilder();

            sb.AppendLine("==================================================");
            sb.AppendLine($"Time: {DateTimeOffset.Now:O}");
            sb.AppendLine($"Source: {source}");
            sb.AppendLine($"AppData: {Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}");
            sb.AppendLine($"OS: {Environment.OSVersion}");
            sb.AppendLine($".NET: {Environment.Version}");
            sb.AppendLine($"64-bit process: {Environment.Is64BitProcess}");

            if (!string.IsNullOrWhiteSpace(extra))
            {
                sb.AppendLine();
                sb.AppendLine("Extra:");
                sb.AppendLine(extra);
            }

            if (exception != null)
            {
                sb.AppendLine();
                sb.AppendLine("Exception:");
                sb.AppendLine(exception.ToString());

                var inner = exception.InnerException;
                var depth = 1;

                while (inner != null)
                {
                    sb.AppendLine();
                    sb.AppendLine($"Inner Exception #{depth}:");
                    sb.AppendLine(inner.ToString());

                    inner = inner.InnerException;
                    depth++;
                }
            }

            sb.AppendLine();

            lock (LockObject)
            {
                File.AppendAllText(LogFilePath, sb.ToString());
            }
        }
        catch
        {
            // Logging darf niemals selbst die App crashen.
        }
    }
}