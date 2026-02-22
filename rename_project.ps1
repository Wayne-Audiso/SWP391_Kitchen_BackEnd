$root = "d:\Backend"
$extensions = @(".cs", ".csproj", ".sln", ".json", ".xml", ".config", ".md", ".yml", ".yaml", ".js", ".html", ".css", ".dockerfile")

# Replacements
$replacements = @{
    "N-Tier" = "BackendSWP391";
    "N_Tier" = "BackendSWP391"
}

# Function to update file content
function Update-FileContent ($filePath) {
    $content = Get-Content -Path $filePath -Raw
    $originalContent = $content
    
    foreach ($key in $replacements.Keys) {
        if ($content -match $key) {
            $content = $content.Replace($key, $replacements[$key])
        }
    }
    
    if ($content -ne $originalContent) {
        Set-Content -Path $filePath -Value $content -Encoding UTF8
        Write-Host "Updated content: $filePath"
    }
}

# 1. Update Content
Get-ChildItem -Path $root -Recurse -File | Where-Object { 
    $ext = $_.Extension.ToLower()
    if ($extensions -contains $ext -or $_.Name -eq "Dockerfile") {
        return $true
    }
    return $false
} | ForEach-Object {
    if ($_.FullName -notmatch "\\.git\\" -and $_.FullName -notmatch "\\bin\\" -and $_.FullName -notmatch "\\obj\\" -and $_.FullName -notmatch "\\.vs\\") {
        Update-FileContent $_.FullName
    }
}

# 2. Rename Files
Get-ChildItem -Path $root -Recurse -File | Where-Object { 
    $_.FullName -notmatch "\\.git\\" -and $_.FullName -notmatch "\\bin\\" -and $_.FullName -notmatch "\\obj\\" -and $_.FullName -notmatch "\\.vs\\"
} | ForEach-Object {
    $newName = $_.Name
    foreach ($key in $replacements.Keys) {
        if ($newName -match $key) {
            $newName = $newName.Replace($key, $replacements[$key])
        }
    }
    
    if ($newName -ne $_.Name) {
        $newPath = Join-Path $_.DirectoryName $newName
        Rename-Item -Path $_.FullName -NewName $newName
        Write-Host "Renamed file: $($_.FullName) -> $newPath"
    }
}

# 3. Rename Directories (Bottom-up)
# Getting all directories first, sorting by length descending ensures child directories come before parents
$dirs = Get-ChildItem -Path $root -Recurse -Directory | Where-Object { 
    $_.FullName -notmatch "\\.git\\" -and $_.FullName -notmatch "\\bin\\" -and $_.FullName -notmatch "\\obj\\" -and $_.FullName -notmatch "\\.vs\\"
} | Sort-Object -Property FullName -Descending

foreach ($dir in $dirs) {
    if (-not (Test-Path $dir.FullName)) { continue } # Skip if already renamed/path changed
    
    $newName = $dir.Name
    foreach ($key in $replacements.Keys) {
        if ($newName -match $key) {
            $newName = $newName.Replace($key, $replacements[$key])
        }
    }
    
    if ($newName -ne $dir.Name) {
        $newPath = Join-Path $dir.Parent.FullName $newName
        Rename-Item -Path $dir.FullName -NewName $newName
        Write-Host "Renamed directory: $($dir.FullName) -> $newPath"
    }
}
