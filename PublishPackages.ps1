try{
  #Move to script directory (when starting from VSCode)
  Set-Location (Split-Path $script:MyInvocation.MyCommand.Path)

  #Find package version:
  [Microsoft.PowerShell.Commands.MatchInfo] $PackageVersionMatch = Select-String -Path "Directory.Build.targets"  -Pattern "<PackageVersion>(.*)</PackageVersion>"
  if ($null -eq $PackageVersionMatch) {
    #Should not happen.
    Write-Host -ForegroundColor Red "Could not determine PackageVersion"
    Read-Host -Prompt "Please press enter"
    exit
  }
  [string] $PackageVersion = $PackageVersionMatch.Matches[0].Groups[1].Value

  Write-Host "Publish version $PackageVersion to: "
  Write-Host "  1) Nuget integration enviromment (https://int.nugettest.org/)"
  Write-Host "  2) Nuget (https://www.nuget.org/)"
  [string] $PublishTargetSelection = Read-Host "Please enter number"
  [string] $PublishServer = ""
  if ($PublishTargetSelection -eq "1") {
    $PublishServer = "https://apiint.nugettest.org/v3/index.json"
  }
  elseif ($PublishTargetSelection -eq "2") {
    $PublishServer = "https://api.nuget.org/v3/index.json"
  }
  else {
    Write-Host -ForegroundColor Red "Invalid selection: $PublishTargetSelection"
    Read-Host -Prompt "Please press enter"
    exit
  }

  [string] $ApiKey = Read-Host "Enter API key"

  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.Core.$PackageVersion.nupkg", "--api-key $apiKey", "--source $PublishServer" -Wait -NoNewWindow
  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.OAuth1.$PackageVersion.nupkg", "--api-key $apiKey", "--source $PublishServer" -Wait -NoNewWindow
  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.OAuth1.WinForms.$PackageVersion.nupkg", "--api-key $apiKey", "--source $PublishServer" -Wait -NoNewWindow
  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.OAuth2.$PackageVersion.nupkg", "--api-key $apiKey", "--source $PublishServer" -Wait -NoNewWindow
  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.OAuth2.WinForms.$PackageVersion.nupkg", "--api-key $apiKey", "--source $PublishServer" -Wait -NoNewWindow

  Read-Host -Prompt "Please press enter"
}
catch [Exception]
{
  Write-Host -ForegroundColor Red $_.Exception | Format-List -Force
  Write-Output $_.InvocationInfo
  Read-Host -Prompt "Please press enter"
}
