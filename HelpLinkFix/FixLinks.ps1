param(
    [Parameter(Mandatory=$true)]
    [string]$environment,
    [Parameter(Mandatory=$true)]
    [string]$folderPath
)

if ($environment -ne "dev") {
    Write-Host "Only applies to environment. No changes will be made."
    exit
}

$filePattern = "*.htm"

# Get list of HTM files in root directory for exclusion
$rootFiles = Get-ChildItem -Path $folderPath -Filter $filePattern -File | 
    Select-Object -ExpandProperty Name

# Create regex pattern for exclusion (root files and anything containing /)
$excludePattern = $rootFiles -join '|' + '|/'
Write-Host "Excluded patterns: $excludePattern"

Get-ChildItem -Path $folderPath -Filter $filePattern -Recurse | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    
    # regex to exclude:
    # 1. Matches from root files list
    # 2. Self-referencing links starting with #
    # 3. Links already starting with /, http, or ./, or ../
    $newContent = $content -replace 'href="(?![/#"]|http|\.\./)(?!(?:$excludePattern))([^"]+)"', 'href="./$1"'
    
    if ($content -ne $newContent) {
        Write-Host "Updating file: $($_.FullName)" 
        $newContent | Set-Content $_.FullName -NoNewline
    }
}

#