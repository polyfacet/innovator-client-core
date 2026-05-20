public static class ConsoleLog
{
    public static void Line(string markup, bool includeTimestamp = true)
    {       
        if (includeTimestamp)
        {
            // Doubling the brackets [[ ]] prevents Spectre from treating the time as a color tag
            string timestamp = $"[[{DateTime.Now:HH:mm:ss}]]"; 
            AnsiConsole.MarkupLine($"{timestamp} {markup}");
        }
        else
            AnsiConsole.MarkupLine($"{markup}");
    }
}