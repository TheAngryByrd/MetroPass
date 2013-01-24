param(
[string]$AppxPackagePath,
[string]$resultPath,
[string]$scriptPath = $(Split-Path -parent $MyInvocation.MyCommand.path)
)

import-module (join-path $scriptPath TeamCity.ps1)

$lastAppPackage = (gci $AppxPackagePath | ? { $_.PSIsContainer } | sort name )[-1].fullname

$dir =  get-childitem $lastAppPackage

$appxfile = ($dir | where {$_.extension -eq ".appx"})[0]

$cert = "C:\Program Files (x86)\Windows Kits\8.0\App Certification Kit\appcert"
$guid = [guid]::NewGuid().ToString() + ".xml"
$reportPath = $resultPath + $guid

cmd /c $cert  reset
cmd /c $cert test -apptype metrostyle -AppxPackagePath $appxfile.fullname -reportoutputpath $reportPath

[xml]$report = Get-Content $reportPath 
$result = $report.REPORT.OVERALL_RESULT