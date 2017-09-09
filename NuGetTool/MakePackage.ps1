param($projectName)

process
{
	msbuild ../Dev/${projectName}/${projectName}.csproj /p:WarningLevel=2 /p:Configuration=Release
	nuget pack "${projectName}/${projectName}.nuspec"
	mv -Force "${projectName}.*.nupkg" "${projectName}/"
}