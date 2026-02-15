@REM delete all packages:
del Packages\*.nupkg

@REM "dotnet pack" does not work:
@REM    MSB3823: Non-string resources require the property GenerateResourceUsePreserializedResources to be set to true.
@REM    MSB3822: Non-string resources require the System.Resources.Extensions assembly at runtime, but it was not found in this project's references
@REM See https://github.com/dotnet/msbuild/issues/5787
@REM Workaround: use "msbuild" instead

@REM place all packages in root subdir "Packages".
"C:\Program Files\Microsoft Visual Studio\18\Professional\MSBuild\Current\Bin\amd64\MSBuild.exe" -t:clean,pack /v:m /p:Configuration=Release /p:PackageOutputPath=..\Packages
@pause