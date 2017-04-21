$version = ./IncrementVersion
msbuild ../Dev/Nac.Altseed/Nac.Altseed.csproj /p:WarningLevel=2 /p:Configuration=Release
cp ..\Dev\Nac.Altseed\bin\Release\Nac.Altseed.dll ../Package/lib/
cp ..\Dev\Nac.Altseed\bin\Release\Nac.Altseed.xml ../Package/lib/
nuget pack ../Package/Nac.Altseed.nuspec
$version