try{
  [string] $ApiKey = Read-Host "Enter API key"

  #TODO: find package version
  [string] $PackageVersion = "0.1.0-Beta1"

  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.Core.$PackageVersion.nupkg", "--api-key $apiKey", "--source https://apiint.nugettest.org/v3/index.json" -Wait -NoNewWindow
  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.OAuth1.$PackageVersion.nupkg", "--api-key $apiKey", "--source https://apiint.nugettest.org/v3/index.json" -Wait -NoNewWindow
  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.OAuth1.WinForms.$PackageVersion.nupkg", "--api-key $apiKey", "--source https://apiint.nugettest.org/v3/index.json" -Wait -NoNewWindow
  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.OAuth2.$PackageVersion.nupkg", "--api-key $apiKey", "--source https://apiint.nugettest.org/v3/index.json" -Wait -NoNewWindow
  Start-Process -FilePath "dotnet.exe" -ArgumentList "nuget", "push", "Packages\XbyOpenApi.OAuth2.WinForms.$PackageVersion.nupkg", "--api-key $apiKey", "--source https://apiint.nugettest.org/v3/index.json" -Wait -NoNewWindow

  Read-Host -Prompt "Please press enter"
}
catch [Exception]
{
  Write-Host -ForegroundColor Red $_.Exception | Format-List -Force
  Write-Output $_.InvocationInfo
  Read-Host -Prompt "Please press enter"
}
