<#
.SYNOPSIS
    Downloading files by URL.
.DESCRIPTION
    Primary use is to download chapter pages obtained through the client.
.EXAMPLE
    PS C:\>./download.ps1 -urls (.\Client.exe get pages -u <chapter url> | where {$_ -like 'http*'}) -outputDir <output directory>
    Passes a chapter URL to the client, then fetches only the page URLs from the output and passes them to the script, which downloads them into the output directory.
.INPUTS
    URLs, output directory
.OUTPUTS
    files
.NOTES
    :)
#>

[CmdletBinding()]
param (
    # Specifies an download urls.
    [Parameter(Mandatory = $true,
        ValueFromPipeline = $true,
        ValueFromPipelineByPropertyName = $true,
        HelpMessage = "Download urls.")]
    [ValidateNotNullOrEmpty()]
    [string[]]
    $urls,
    # Specifies a path to output location.
    [Parameter(Mandatory = $true,
        ValueFromPipeline = $true,
        ValueFromPipelineByPropertyName = $true,
        HelpMessage = "Path to output location.")]
    [ValidateNotNullOrEmpty()]
    [string]
    $outputDir
)

mkdir $outputDir -Force

foreach ($url in $urls) {
    $filename = [System.IO.Path]::GetFileName([System.Uri]::new($url).LocalPath)
    $output = ".\$outputDir\$filename"
    Invoke-WebRequest $url -OutFile $output
}