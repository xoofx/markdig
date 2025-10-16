param (
    [string]$configuration = $null
)

Set-StrictMode -Version Latest

$libFuzzer = "libfuzzer-dotnet-windows.exe"
$outputDir = "bin"

function Get-LibFuzzer {
    param (
        [string]$Path
    )

    $libFuzzerUrl = "https://github.com/Metalnem/libfuzzer-dotnet/releases/download/v2025.05.02.0904/libfuzzer-dotnet-windows.exe"
    $expectedHash = "17af5b3f6ff4d2c57b44b9a35c13051b570eb66f0557d00015df3832709050bf"

    Write-Output "Downloading libFuzzer from $libFuzzerUrl..."

    try {
        $tempFile = "$Path.tmp"
        Invoke-WebRequest -Uri $libFuzzerUrl -OutFile $tempFile -UseBasicParsing

        $downloadedHash = (Get-FileHash -Path $tempFile -Algorithm SHA256).Hash

        if ($downloadedHash -eq $ExpectedHash) {
            Move-Item -Path $tempFile -Destination $Path -Force
            Write-Output "libFuzzer downloaded successfully to $Path"
        }
        else {
            Write-Error "Hash validation failed."
            Remove-Item -Path $tempFile -Force -ErrorAction SilentlyContinue
            exit 1
        }
    }
    catch {
        Write-Error "Failed to download libFuzzer: $($_.Exception.Message)"
        Remove-Item -Path $tempFile -Force -ErrorAction SilentlyContinue
        exit 1
    }
}

# Check if libFuzzer exists, download if not
if (-not (Test-Path $libFuzzer)) {
    Get-LibFuzzer -Path $libFuzzer
}

$toolListOutput = dotnet tool list --global sharpFuzz.CommandLine 2>$null
if (-not ($toolListOutput -match "sharpfuzz")) {
    Write-Output "Installing sharpfuzz CLI"
    dotnet tool install --global sharpFuzz.CommandLine
}

if (Test-Path $outputDir) {
    Remove-Item -Recurse -Force $outputDir
}

if ($configuration -eq $null) {
    $configuration = "Debug"
}

dotnet publish -c $configuration -o $outputDir

$project = Join-Path $outputDir "Markdig.Fuzzing.dll"

$fuzzingTarget = Join-Path $outputDir "Markdig.dll"

Write-Output "Instrumenting $fuzzingTarget"
& sharpfuzz $fuzzingTarget

if ($LastExitCode -ne 0) {
    Write-Error "An error occurred while instrumenting $fuzzingTarget"
    exit 1
}

New-Item -ItemType Directory -Force -Path corpus | Out-Null

$libFuzzerArgs = @("--target_path=dotnet", "--target_arg=$project", "-timeout=10", "corpus")

# Add any additional arguments passed to the script
if ($args) {
    $libFuzzerArgs += $args
}

Write-Output "Starting libFuzzer with arguments: $libFuzzerArgs"
& ./$libFuzzer @libFuzzerArgs