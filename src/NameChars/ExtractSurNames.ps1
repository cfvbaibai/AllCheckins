$webClient = New-Object net.WebClient
$htmlSource = $webClient.DownloadString("http://www.resgain.net/index.html")
$pattern = "http://(?<SurName>[^.]+).resgain.net"
$matches = ($htmlSource | Select-String -AllMatches $pattern).Matches
$resultFilePath = "SurNames.txt"
if (Test-Path $resultFilePath) {
    Remove-Item $resultFilePath
}
$matches | ForEach-Object {
    $_.Groups['SurName'].Value | Out-File $resultFilePath -Append
}