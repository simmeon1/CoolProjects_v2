dotnet stryker
$resultsPath = $PSScriptRoot + "\StrykerOutput"
$latestResults = gci $resultsPath | ? { $_.PSIsContainer } | sort CreationTime -desc | select -f 1
$report = "$($resultsPath)\$($latestResults)\reports\mutation-report.html"
$destination = $PSScriptRoot + "\MutationCoverageReport\"
New-Item -ItemType Directory -Force -Path $destination
Copy-Item $report -Destination $destination
Remove-Item $resultsPath -Recurse