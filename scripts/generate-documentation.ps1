$docFolder = "_site"
$documentationBranch = "documentation" #TODO change to gh-pages

Push-Location -StackName ScriptFolder
Set-Location ../$docFolder

If(!(Test-Path -Path $docFolder))
{
    git clone https://github.com/xxMUROxx/Mairegger.Printing . -b $documentationBranch
}
else
{
    $currentBranch= &git rev-parse --abbrev-ref HEAD 

    if($currentBranch -ne $documentationBranch)
    {
        git checkout $documentationBranch
    }
    git pull
}

Set-Location ..

docfx

Set-Location $docFolder

Remove-Item manifest.json
Get-ChildItem * -Include *.yml | Remove-Item

git add .
git commit -m "Update $documentationBranch"
git push

Pop-Location -StackName ScriptFolder