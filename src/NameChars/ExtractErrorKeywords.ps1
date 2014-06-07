$scriptDir = Split-Path $myInvocation.MyCommand.Path
$logPath = "$scriptDir\log.log"
if (-not (Test-Path $logPath)) {
    "Cannot find log file at {0}. Exit!" -f $logPath | Write-Host -ForegroundColor Red
    exit 1
}
$logs = Get-Content $logPath -Encoding UTF8
$resultFilePath = "$scriptDir\error.txt"
if (Test-Path $resultFilePath) { Remove-Item $resultFilePath }
$matchInfo = $logs | Select-String -AllMatches "\[keyword = (?<Keyword>[^\]]+)\]"
foreach ($match in $matchInfo.Matches) {
    $match.Groups["Keyword"].Value | Out-File $resultFilePath -Append 
}