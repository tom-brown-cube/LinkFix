using System.Text.RegularExpressions;
using System.IO;

static void SearchInFile(string filePath, StreamWriter logWriter)
{
    // Regex pattern to match href starting with an alphabetic character, excluding "mailto:"
    string pattern = "<a href=\"(?!mailto:)[a-zA-Z]";
    string[] lines = File.ReadAllLines(filePath);

    bool printHeader = false; // Flag to track if there are any matches

    for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
    {
        string line = lines[lineNumber];
        MatchCollection matches = Regex.Matches(line, pattern);

        foreach (Match match in matches)
        {
            if (!printHeader)
            {
                // Write the filename and separator only once when the first match is found
                logWriter.WriteLine($"File: {filePath}");
                logWriter.WriteLine(new string('_', 80)); // Separator line of underscores
                printHeader = true;
            }

            // Write match details in the new format
            logWriter.WriteLine($"Line: {lineNumber + 1} - Position: {match.Index}");
            logWriter.WriteLine(line.Trim());
        }
    }
}

string testProject = @"Work\MadCap\Help-20250408";
string rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), testProject);

if (!Directory.Exists(rootPath))
{
    Console.WriteLine("Enter the root directory path to search:");
    rootPath = Console.ReadLine();
    if (!Directory.Exists(rootPath))
    {
        Console.WriteLine("Directory does not exist!");
        return;
    }
}

string logFilePath = Path.Combine(rootPath, "HelpLinkFixResults.log");
if (File.Exists(logFilePath))
{
    File.Delete(logFilePath);
}
try
{
    // Open the log file for writing
    using (StreamWriter logWriter = new StreamWriter(logFilePath, false))
    {
        // Get all .htm files in the directory and subdirectories
        string[] files = Directory.GetFiles(rootPath, "*.htm", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            SearchInFile(file, logWriter);
        }
    }

    Console.WriteLine($"Results have been written to: {logFilePath}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

