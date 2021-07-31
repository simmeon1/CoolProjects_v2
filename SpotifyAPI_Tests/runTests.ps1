dotnet test --collect:"XPlat Code Coverage"
$testResultsPath = $PSScriptRoot + "\TestResults"
$latestResults = gci $testResultsPath | ? { $_.PSIsContainer } | sort CreationTime -desc | select -f 1
$reportXml = "$($testResultsPath)\$($latestResults)\coverage.cobertura.xml"
$path = $env:USERPROFILE + "\.nuget\packages\reportgenerator\4.8.12\tools\net5.0\ReportGenerator.dll" 
$command1 = "-reports:" + $reportXml
$command2 = "-targetdir:" + $testResultsPath + "\coveragereport"
dotnet $path $command1 $command2 -reporttypes:Html