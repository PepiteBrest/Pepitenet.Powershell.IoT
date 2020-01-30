param($action)

Write-Host ""
Write-Host "+--------------------------+" -ForegroundColor Cyan
Write-Host "| Pepitenet.Powershell.IoT |" -ForegroundColor Cyan
Write-Host "+--------------------------+" -ForegroundColor Cyan
Write-Host ""

function doBuild($loc)
{
    # Generates dependencies
    Write-Host "Generating Dependencies" -ForegroundColor Cyan
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\raspberryio\src\Unosquare.RaspberryIO"
    Write-Host "Generating Unosquare.RaspberryIO project" -ForegroundColor DarkCyan
    & dotnet build -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\raspberryio\src\Unosquare.RaspberryIO.Abstractions"
    Write-Host "Generating Unosquare.RaspberryIO.Abstractions project" -ForegroundColor DarkCyan
    & dotnet build -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\raspberryio\src\Unosquare.RaspberryIO.Peripherals"
    Write-Host "Generating Unosquare.RaspberryIO.Peripherals project" -ForegroundColor DarkCyan
    & dotnet build -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\swan\src\Swan"
    Write-Host "Generating Swan project" -ForegroundColor DarkCyan
    & dotnet build -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\swan\src\Swan.Lite"
    Write-Host "Generating Swan.Lite project" -ForegroundColor DarkCyan
    & dotnet build -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\wiringpi-dotnet\src\Unosquare.WiringPi"
    Write-Host "Generating Unosquare.WiringPi project" -ForegroundColor DarkCyan
    & dotnet build -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location


    # Generates Pepitenet.Powershell.IoT
    Push-Location $location\src
    Write-Host "Generating Pepitenet.Powershell.IoT project" -ForegroundColor Cyan
    & dotnet build -c Release -o $OutputFolder | Out-Null
    Pop-Location
    
    Write-Host "Remove unnecessary files" -ForegroundColor Cyan
    Write-Host "Remove pdb files" -ForegroundColor DarkCyan
    Get-ChildItem -Path $OutputFolder -Filter "*.pdb" | Remove-Item
    Write-Host "Remove json files" -ForegroundColor DarkCyan
    Get-ChildItem -Path $OutputFolder -Filter "*.json" | Remove-Item
    Write-Host "Remove xml files" -ForegroundColor DarkCyan
    Get-ChildItem -Path $OutputFolder -Filter "*.xml" | Remove-Item
}

function doClean($loc)
{
    # Generates dependencies
    Write-Host "Cleaning Dependencies" -ForegroundColor Cyan
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\raspberryio\src\Unosquare.RaspberryIO"
    Write-Host "Cleaning Unosquare.RaspberryIO project" -ForegroundColor DarkCyan
    & dotnet clean -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\raspberryio\src\Unosquare.RaspberryIO.Abstractions"
    Write-Host "Cleaning Unosquare.RaspberryIO.Abstractions project" -ForegroundColor DarkCyan
    & dotnet clean -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\raspberryio\src\Unosquare.RaspberryIO.Peripherals"
    Write-Host "Cleaning Unosquare.RaspberryIO.Peripherals project" -ForegroundColor DarkCyan
    & dotnet clean -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\swan\src\Swan"
    Write-Host "Cleaning Swan project" -ForegroundColor DarkCyan
    & dotnet clean -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\swan\src\Swan.Lite"
    Write-Host "Cleaning Swan.Lite project" -ForegroundColor DarkCyan
    & dotnet clean -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location
    Push-Location "C:\Users\mathi\OneDrive\Documents\PowerShell\Pepitenet.Powershell.IoT\submodule\wiringpi-dotnet\src\Unosquare.WiringPi"
    Write-Host "Cleaning Unosquare.WiringPi project" -ForegroundColor DarkCyan
    & dotnet clean -c Release -o $OutputFolder -f netstandard2.0 | Out-Null
    Pop-Location

    # Generates Pepitenet.Powershell.IoT
    Push-Location $location\src
    Write-Host "Cleaning Pepitenet.Powershell.IoT project" -ForegroundColor Cyan
    & dotnet clean -c Release -o $OutputFolder | Out-Null
    Pop-Location
}

# Init submodules
git submodule update --init

$location = Get-Location
$OutputFolder = "$location\Output"

switch($action)
{
    "build" {
        Write-Host "**** GENERATING PROJECT ****" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "Creating Output Folder" -ForegroundColor Cyan
        if(Test-Path $OutputFolder)
        {
            Remove-Item -Path $OutputFolder -Recurse -Force
        }
        New-Item -ItemType Directory -Path $OutputFolder | Out-Null

        doBuild $location
        
        Copy-Item .\Pepitenet.Powershell.IoT.psd1 $OutputFolder

    }
    "clean" {
        Write-Host "**** CLEANING PROJECT ****" -ForegroundColor Cyan
        Write-Host ""
        doClean $location

        Write-Host "Removing Output folder" -ForegroundColor Cyan
        if(Test-Path $OutputFolder)
        {
            Remove-Item -Path $OutputFolder -Recurse -Force
        }
    }
}
Write-Host ""
