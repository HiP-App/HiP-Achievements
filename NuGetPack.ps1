$csproj = (ls *.Sdk\*.csproj).FullName
Switch ("$env:Build_SourceBranchName")
{
    "master" { dotnet pack "$csproj" -o . }
    "develop" { dotnet pack "$csproj" -o . --version-suffix "develop" }
	"iss-hipcms-876" { dotnet pack "$csproj" -o . --version-suffix "test" }
    default { exit }
}
$nupkg = (ls *.Sdk\*.nupkg).FullName
dotnet nuget push "$nupkg" -k "$env:MyGetKey" -s "$env:NuGetFeed"
$LASTEXITCODE = 0