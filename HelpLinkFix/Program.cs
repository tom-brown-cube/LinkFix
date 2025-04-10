﻿using System.Text.RegularExpressions;
using System.IO;

static int SearchInFile(string filePath, string baseFolder, 
    StreamWriter logWriter, List<string> excludedDirectLinks)
{
    // Regex pattern to match href starting with an alphabetic character, excluding "mailto:"
    string pattern = "<a href=\"(?!mailto:)[a-zA-Z]";
    string[] lines = File.ReadAllLines(filePath);

    // Calculate the relative folder of filePath
    string relativeFolder = Path.GetRelativePath(baseFolder, Path.GetDirectoryName(filePath) ?? string.Empty);

    bool printHeader = false; // Flag to track if there are any matches

    int directLinkCount = 0;
    for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
    {
        string line = lines[lineNumber];
        MatchCollection matches = Regex.Matches(line, pattern);

        foreach (Match match in matches)
        {
            if (!printHeader)
            {
                // Write the filename and separator only once when the first match is found
                logWriter.WriteLine($"File: {relativeFolder}\\{Path.GetFileName(filePath)}");
                logWriter.WriteLine(new string('_', 80)); // Separator line of underscores
                printHeader = true;
            }

            // Extract the href value from the match
            string hrefValue = ExtractHrefValue(line);

            // Check if the href is a direct link to an .htm file (no folder structure)
            bool isDirectLink = hrefValue.EndsWith(".htm", StringComparison.OrdinalIgnoreCase) &&
                                !hrefValue.Contains('/') &&
                                !excludedDirectLinks.Contains(hrefValue, StringComparer.OrdinalIgnoreCase); ;
            if (isDirectLink)
            {
                // Write match details with the marker if it's a direct link
               // string marker = isDirectLink ? " ***>>" : string.Empty;
                logWriter.WriteLine($"Line: {lineNumber + 1} - Position: {match.Index}");
                logWriter.WriteLine(line.Trim());
                directLinkCount++;
            }
        }
    }

    return directLinkCount;
}
// Helper method to extract the href value from the match
static string ExtractHrefValue(string matchValue)
{
    // Extract the value inside the href attribute
    var hrefMatch = Regex.Match(matchValue, "href=\"([^\"]+)\"");
    return hrefMatch.Success ? hrefMatch.Groups[1].Value : string.Empty;
}


string testProject = @"Work\MadCap\Help-20250408\RGPHELPPORTAL";
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
    int totalDirectLinks = 0;

    // Hardcoded list of excluded direct links
    List<string> excludedDirectLinks = new List<string> { "home.htm", "search.htm", "glossary.htm" };
    // read all htm files in the rootPath and put the filenames into excludedDirectLinks
    // Read all .htm files in the rootPath and add their filenames to excludedDirectLinks
    string[] allHtmFiles = Directory.GetFiles(rootPath, "*.htm", SearchOption.TopDirectoryOnly);
    foreach (string file in allHtmFiles)
    {
        string fileName = Path.GetFileName(file);
        if (!excludedDirectLinks.Contains(fileName, StringComparer.OrdinalIgnoreCase))
        {
            excludedDirectLinks.Add(fileName);
        }
    }


    // Open the log file for writing
    using (StreamWriter logWriter = new StreamWriter(logFilePath, false))
    {
        // Get all .htm files in the directory and subdirectories
        string[] files = Directory.GetFiles(rootPath, "*.htm", SearchOption.AllDirectories);


        foreach (string file in files)
        {
            var directLinks =    SearchInFile(file, rootPath, logWriter, excludedDirectLinks);
            if (directLinks > 0)
            {
                logWriter.WriteLine($"DirectLinks {directLinks} in File: {file}");
                totalDirectLinks += directLinks;
            }
        }
    }

    Console.WriteLine($"Total Direct Links: {totalDirectLinks}   Results have been written to: {logFilePath}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();

