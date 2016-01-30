param($versionInc = -1)

process
{
    $spec = [xml](Get-Content "..\Package\Nac.Altseed.nuspec")
    $version = $spec.package.metadata.version.Split(".") | foreach { [int]::Parse($_) }
    
    if($versionInc -eq -1)
    {
        [System.Console]::Write("input version position to increment: ")
        $versionInc = [int]::Parse([System.Console]::ReadLine())
    }
    
    if($versionInc -ne -1)
    {
        $version[$versionInc]++;
        for($i = $versionInc + 1; $i -lt $version.Length; ++$i)
        {
            $version[$i] = 0;
        }   
    }
    
    $spec.package.metadata.version = [string]::Join(".", $version)
    $spec.Save((Get-Location).Path + "/../Package/Nac.Altseed.nuspec")
    
    $spec.package.metadata.version
}