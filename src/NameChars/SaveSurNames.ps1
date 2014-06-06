$scriptDir = Split-Path $MyInvocation.MyCommand.Path

$resultFilePath = "$scriptDir\SurNames.csv"
if (Test-Path $resultFilePath) { Remove-Item $resultFilePath }

$givenNamesDir = "$scriptDir\GivenNames"
"SurName, Weight" | Out-File -Append -FilePath $resultFilePath
foreach ($file in (Get-ChildItem $givenNamesDir)) {
    "SurName = {0,-16}, Size = {1}" -f $file.BaseName, $file.Length
    ("{0},{1}" -f $file.BaseName, $file.Length) | Out-File -Append -FilePath $resultFilePath
}