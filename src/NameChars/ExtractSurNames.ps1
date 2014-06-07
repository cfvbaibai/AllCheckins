Add-Type -AssemblyName System.Web

$scriptDir = Split-Path $MyInvocation.MyCommand.Path

$webClient = New-Object net.WebClient
$htmlSource = $webClient.DownloadString("http://www.resgain.net/index.html")
#<a href="http://wang.resgain.net" title="&#29579;&#22995;&#26063;&#35889;#">&#29579;&#22995;&#20043;&#23478;</a>
$pattern = '<a (.*)href="http://(?<SurName>[^.]+).resgain.net" title="[^"]+">(?<SurNameText>[^<]+)</a>'
$matches = ($htmlSource | Select-String -AllMatches $pattern).Matches

$resultFilePath = "$scriptDir\SurNames.txt"
if (Test-Path $resultFilePath) {
    Remove-Item $resultFilePath
}

"SurNameRoma,SurNameChinese" | Out-File $resultFilePath -Append

$matches | ForEach-Object {
    $surNameText = [System.Web.HttpUtility]::HtmlDecode($_.Groups['SurNameText'])
    if ($surNameText[1] -eq '姓') {
        $surNameChineseText = $surNameText.Substring(0, 1)
    } else {
        $surNameChineseText = $surNameText.Substring(0, 2)
    }
    $surNameRoma = $_.Groups['SurName'].Value
    if ($surNameRoma.Contains(')')) { return; }
    ("{0},{1}" -f $surNameRoma, $surNameChineseText) | Out-File $resultFilePath -Append
}