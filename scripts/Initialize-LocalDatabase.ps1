$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$envPath = Join-Path $repoRoot '.env'
if (-not (Test-Path -LiteralPath $envPath)) {
    $bytes = New-Object byte[] 32
    $rng = [Security.Cryptography.RandomNumberGenerator]::Create()
    try { $rng.GetBytes($bytes) } finally { $rng.Dispose() }
    $password = [BitConverter]::ToString($bytes).Replace('-', '').ToLowerInvariant()
    Set-Content -LiteralPath $envPath -Value "POSTGRES_PASSWORD=$password"
}
$line = Get-Content -LiteralPath $envPath | Where-Object { $_ -match '^POSTGRES_PASSWORD=' } | Select-Object -First 1
if (-not $line) { throw 'POSTGRES_PASSWORD is missing in .env' }
$password = $line.Substring('POSTGRES_PASSWORD='.Length)
if ($password -notmatch '^[a-zA-Z0-9_-]{16,}$') {
    throw 'Use at least 16 letters, digits, underscores or hyphens for the local password.'
}
$configPath = Join-Path $repoRoot 'Tilskuddsapp/appsettings.Local.json'
if (-not (Test-Path -LiteralPath $configPath)) {
    @{ConnectionStrings=@{Postgres="Host=localhost;Port=5433;Database=tilskuddsapp;Username=tilskuddsapp;Password=$password"}} |
        ConvertTo-Json | Set-Content -LiteralPath $configPath -Encoding utf8
}
Write-Output 'Local configuration ready. Run docker compose up -d --build from the repository root.'
