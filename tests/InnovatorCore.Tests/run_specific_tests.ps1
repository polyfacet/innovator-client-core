# $testName = "DataModel_Part_ECO_Demo_Test"
$testName = "DataModel_Packages_Demo_Test"
Write-Host -ForegroundColor Cyan "Running test like $testName"
dotnet test --filter DisplayName~$testName --logger "console;verbosity=detailed" .\InnovatorCore.Tests.csproj --no-restore