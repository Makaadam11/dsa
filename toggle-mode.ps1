param([ValidateSet('hackerrank', 'normal')][string]$Mode)

$vscode = Join-Path $PSScriptRoot '.vscode'
$target = Join-Path $vscode 'settings.json'

if (-not $Mode) {
    $current = if ((Get-Content $target -Raw) -match 'tryb HackerRank') { 'hackerrank' } else { 'normal' }
    $Mode = if ($current -eq 'hackerrank') { 'normal' } else { 'hackerrank' }
}

Copy-Item (Join-Path $vscode "settings.$Mode.json") $target -Force
Write-Host "Tryb: $Mode (przeladuj okno: Ctrl+Shift+P > Reload Window)"
