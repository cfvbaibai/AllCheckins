$scriptDir = Split-Path $MyInvocation.MyCommand.Path

$givenNamesFolder = "$scriptDir\GivenNames"
#Set-Location SQLSERVER:\SQL\CFVBAIBAI-S01\DEFAULT\Databases\kaifang
$givenNames = @{}

#"USE kaifang" | Out-File -Append -FilePath $sqlScriptPath
$i = 0;
$givenNameFiles = Get-ChildItem $givenNamesFolder
foreach ($givenNameFile in $givenNameFiles) {
    ++$i
    #if ($i -gt 3) { break }
    #$sqlScriptPath = "$scriptDir\InsertGivenNames\$($_.BaseName).sql"
    #$sqlScriptPath
    "Loading $($givenNameFile.BaseName)..."
    
    #if (Test-Path $sqlScriptPath) { Remove-Item $sqlScriptPath }
    Get-Content $givenNameFile.FullName | ForEach-Object {
        #"EXEC dbo.sp_kf_NewGivenName_1 @nvc_given_name = '$_'" | Out-File -Append -FilePath $sqlScriptPath 
        $givenNames[$_] = $true
    }
}

"$($givenNames.Count) distinct entries found!"

$resultFilePath = "$scriptDir\DistinctGivenNames.txt"
if (Test-Path $resultFilePath) { Remove-Item $resultFilePath }
$i = 0
$givenNames.Keys | ForEach-Object {
    ++$i
    "Writing to file... {0}/{1} {2:f2}%" -f $i, $givenNames.Count, ($i * 100 / $givenNames.Count) | Write-Host -NoNewline 
    $_ | Out-File -Append -FilePath $resultFilePath
}
<#
Get-ChildItem $givenNamesFolder | ForEach-Object {
    Get-Content $_ | ForEach-Object {
        Write-Host $_
    }
}
#>