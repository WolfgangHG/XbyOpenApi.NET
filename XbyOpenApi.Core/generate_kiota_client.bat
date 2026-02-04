@REM generate kiota client
dotnet kiota generate -l CSharp -c XClient -n XbyOpenApi.Core.Client -d https://api.twitter.com/2/openapi.json -o ./Client --exclude-backward-compatible

@REM apply whitespace formatting according to .editorconfig afterwards
dotnet format whitespace --include-generated

@echo Now format all "#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER" blocks that are not touched by the first run.
@echo To do so, change the target framework in XbyOpenApi.Core.csproj to "netstandard2.1":
@echo "     <TargetFramework>netstandard2.1</TargetFramework>"
@echo ""
@echo Save project file and press ENTER to continue.
@pause

dotnet format whitespace --include-generated

@echo Now revert the change to XbyOpenApi.Core.csproj and switch back to "netstandard2.0"

@echo Applying workaround for binary data patch
@REM argument "--directory" is required here, as we are in subdir of the root.
@REM git must be on the path.
git apply --verbose --directory XbyOpenApi.Core kiota_changes.patch

@pause