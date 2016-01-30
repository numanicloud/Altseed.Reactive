$version = ./IncrementVersion
msbuild ../Dev/Nac.Altseed/Nac.Altseed.csproj /p:WarningLevel=2 /p:Configuration=Release
nuget pack ../Package/Nac.Altseed.nuspec
$fileName = "Nac.Altseed." + $version + ".nupkg"
$packagePath = "../Package/" + $fileName
rm $packagePath
mv $fileName $packagePath
copy $packagePath D:\Documents\#MyFiles\LocalNugetFeed\Nac.Altseed\
echo ($fileName + " has been deployed." )
pause