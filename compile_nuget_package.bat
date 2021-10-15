nuget update -self
mkdir ..\NuGetPackages

rem call set_nuget_local_repository_path.bat

rem dotnet pack ActorsCP.csproj -c Release
rem dotnet pack ActorsCP.csproj -c Release --force -o .\
rem         <PackageVersion>$([System.DateTime]::Now.ToString("yyyy.MM.dd.HHmmss"))</PackageVersion>
rem -p:PackageVersion=20.10.07.02
rem dotnet build -p:Version=1.2.3 and dotnet pack -p:PackageVersion=1.2.3.
rem https://github-wiki-see.page/m/Kentico/Home/wiki/Kentico%27s-best-practices-for-.csproj-files

dotnet pack ActorsCP\ActorsCP.csproj  -c Release --include-symbols  -o .\NuGetPackages --force  


