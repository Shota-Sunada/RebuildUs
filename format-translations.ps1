# This script formats all JSON files in the Translations folder.
$translationsPath = Join-Path $PSScriptRoot "RebuildUs\Localization\Translations"

if (-not (Test-Path $translationsPath)) {
    Write-Host "Translations directory not found: $translationsPath" -ForegroundColor Red
    return
}

$jsonFiles = Get-ChildItem -Path $translationsPath -Filter "*.json"

foreach ($file in $jsonFiles) {
    Write-Host "Formatting $($file.Name)..."
    try {
        $rawContent = Get-Content -Path $file.FullName -Raw -Encoding UTF8
        if ([string]::IsNullOrWhiteSpace($rawContent)) {
            continue
        }

        $json = $rawContent | ConvertFrom-Json
        # Serialize with standard PowerShell ConvertTo-Json
        $formatted = $json | ConvertTo-Json -Depth 100

        # PowerShell 5.1 quirks handling:
        # ConvertTo-Json in PS 5.1 does not produce standard indentation.
        # We manually re-indent to 2 spaces for consistency.
        $lines = $formatted -split "`r?`n"
        $newJson = New-Object System.Text.StringBuilder
        $level = 0

        foreach ($line in $lines) {
            $trimmed = $line.Trim()
            if ($trimmed -eq "") { continue }

            # Decrease level if line starts with closing bracket
            if ($trimmed.StartsWith("}") -or $trimmed.StartsWith("]")) {
                $level--
            }

            # Fix double spaces after colons (PS 5.1 quirk)
            if ($trimmed -match '":  ') {
                $trimmed = $trimmed -replace '":  ', '": '
            }

            # Apply indentation (2 spaces per level)
            $indent = "  " * [Math]::Max(0, $level)
            [void]$newJson.AppendLine("$indent$trimmed")

            # Increase level if line ends with opening bracket and doesn't close it on the same line
            if (($trimmed.EndsWith("{") -or $trimmed.EndsWith("[")) -and -not ($trimmed.Contains("}") -or $trimmed.Contains("]"))) {
                $level++
            }
        }

        $finalOutput = $newJson.ToString().TrimEnd()

        # Ensure UTF-8 without BOM (Byte Order Mark)
        $utf8NoBom = New-Object System.Text.UTF8Encoding($false)
        [System.IO.File]::WriteAllText($file.FullName, $finalOutput, $utf8NoBom)
    }
    catch {
        Write-Host "Error formatting $($file.Name): $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "Formatting complete." -ForegroundColor Green
