#0 - all, 1 - unit, 2 - integration
param([String]$testProjectPath, [String]$testType=0)
if ($testType -eq 0) {
    dotnet test --collect:"XPlat Code Coverage"
} elseif ($testType -eq 1) {
    dotnet test --collect:"XPlat Code Coverage" --filter FullyQualifiedName~.UnitTests
} elseif ($testType -eq 2) {
    dotnet test --collect:"XPlat Code Coverage" --filter FullyQualifiedName~.IntegrationTests
}
# $testResultsPath = $PSScriptRoot + "\TestResults"
$testResultsPath = $testProjectPath + "\TestResults"
$latestResults = gci $testResultsPath | ? { $_.PSIsContainer } | sort CreationTime -desc | select -f 1
$reportXml = "$($testResultsPath)\$($latestResults)\coverage.cobertura.xml"
# $path = $env:USERPROFILE + "\.nuget\packages\reportgenerator\4.8.12\tools\net5.0\ReportGenerator.dll" 
# $targetDir = $PSScriptRoot + "\CodeCoverageReport"
$targetDir = $testResultsPath + "\CodeCoverageReport"
New-Item -ItemType Directory -Force -Path $targetDir
$command1 = "-reports:" + $reportXml
$command2 = "-targetdir:" + $targetDir
# dotnet $path $command1 $command2 -reporttypes:Html
reportgenerator $command1 $command2 -reporttypes:Html_Dark
# Remove-Item $testResultsPath -Recurse
$indexFile = $targetDir + "\index.html" 
Invoke-Item $indexFile