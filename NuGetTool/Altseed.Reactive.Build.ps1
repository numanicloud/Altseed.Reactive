param($projectName)

process
{
	msbuild ../Dev/${projectName}/${projectName}.csproj /p:WarningLevel=2 /p:Configuration=Release
	cp -Force "../Dev/${projectName}/bin/Release/${projectName}.dll" "./Release/${projectName}.dll"
	cp -Force "../Dev/${projectName}/bin/Release/${projectName}.xml" "./Release/${projectName}.xml"
}