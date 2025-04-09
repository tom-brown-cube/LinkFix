// See https://aka.ms/new-console-template for more information
using System.Text.RegularExpressions;

static void SearchInFile(string filePath)
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
                // Print the filename and separator only once when the first match is found
                Console.WriteLine($"File: {filePath}");
                Console.WriteLine(new string('_', 80)); // Separator line of underscores
                printHeader = true;
            }

            // Output match details in the new format
            Console.WriteLine($"Line: {lineNumber + 1} - Position: {match.Index}");
            Console.WriteLine(line.Trim());
        }
    }
}


Console.WriteLine("Hello, World!");

Console.WriteLine("Enter the root directory path to search:");
//string rootPath = Console.ReadLine();
string rootPath = @"C:\Users\Tom.Brown.THECONTENTGROUP\Documents\Work\MadCap\Help-20250408";

if (!Directory.Exists(rootPath))
{
    Console.WriteLine("Directory does not exist!");
    return;
}

try
{
    // Get all .htm files in the directory and subdirectories
    string[] files = Directory.GetFiles(rootPath, "*.htm", SearchOption.AllDirectories);

    foreach (string file in files)
    {
        SearchInFile(file);
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();


