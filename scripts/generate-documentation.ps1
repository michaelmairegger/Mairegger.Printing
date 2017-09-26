$currentDirectory = [string](Get-Item -Path '.');

if($currentDirectory.EndsWith("scripts"))
{
    Set-Location ".."
}

cmd /c docfx
