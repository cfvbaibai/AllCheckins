Add-Type -AssemblyName System.Web

$scriptDir = Split-Path $MyInvocation.MyCommand.Path

$pattern = "<a href=""[^""]+"" class=""btn btn-link"">(?<FullName>[^<]+)</a>"
$surNamesFilePath = "$scriptDir\SurNames.txt"

try {
    $webClient = New-Object net.WebClient
    $surNames = Get-Content $surNamesFilePath
    $gendres = 'boys', 'girls'
    $iSurName = 0
    foreach ($surName in $surNames) {
        $givenNamesFilePath = "$scriptDir\GivenNames\{0}.txt" -f $surName
        if (Test-Path $givenNamesFilePath) {
            Remove-Item $givenNamesFilePath
        }
        ++$iSurName
        foreach ($gendre in $gendres) {
            $i = 0
            while ($i -lt 999) {
                ++$i
                $url = "http://{0}.resgain.net/name/{1}_{2}.html" -f $surName, $gendre, $i
                Write-Host -NoNewline ('[{0}/{1}]' -f $iSurName, $surNames.Count)
                Write-Host -NoNewLine "Loading from $url..."
                $htmlSource = $webClient.DownloadString($url)
                $matchInfo = $htmlSource | Select-String -AllMatches $pattern
                $matchCount = $matchInfo.Matches.Count
                if ($matchCount -eq 0) {
                    "No more entries. Next!"
                    break
                }
                $matchInfo.Matches | ForEach-Object {
                    $fullName = [System.Web.HttpUtility]::HtmlDecode($_.Groups["FullName"].Value)
                    if ($fullName.Length -gt 1) {
                        $givenName = $fullName.Substring(1)
                        $givenName | Out-File -FilePath $givenNamesFilePath -Append
                    }
                }
                "{0} entries extracted!" -f $matchCount
            }
        }
    }
} finally {
    $webClient.Dispose()
}