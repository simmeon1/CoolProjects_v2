﻿#For use with Rider Powershell plugin
#0 - all, 1 - unit, 2 - integration
param([String]$testType=0, [String]$runMutation=0)

# Get project folder
$testProjectPath = Get-Location
$testProjectPath -match '.*\w+_Tests'
$testProjectPath = $Matches[0]
Set-Location -Path $testProjectPath

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
$reportXml = "$($latestResults)\coverage.cobertura.xml"
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
Write-Host $indexFile

if ($runMutation -eq 1) {
    $targetDir = $testResultsPath + "\StrykerOutput"
    dotnet stryker --output $targetDir --open-report
}