# Define the path to your project file (e.g., .csproj)
$projectFilePath = "..\src\CTBX.AppHost\CTBX.AppHost.csproj"

# Check if the User Secrets ID is already set
$userSecretsId = (Get-Content $projectFilePath | Select-String -Pattern "<UserSecretsId>(.*?)</UserSecretsId>").Matches.Groups[1].Value

if (-not $userSecretsId) {
    Write-Host "User Secrets ID is not set. Initializing User Secrets..."
    dotnet user-secrets init --project $projectFilePath
}

# Define an array of user secrets keys
$userSecretsKeys = @(
  "Parameters:WebCockpitClientSecret",
  "Parameters:WebCockpitClientId",      
  "Parameters:BackendClientSecret",
  "Parameters:BackendClientId",
  "Parameters:DemoUserPassword",
  "Parameters:DemoUserName")

# Loop through each key and prompt for the value
foreach ($key in $userSecretsKeys) {
    $value = Read-Host "Enter the value for '$key'"
    dotnet user-secrets set $key $value --project $projectFilePath
}

Write-Host "User secrets have been set."
