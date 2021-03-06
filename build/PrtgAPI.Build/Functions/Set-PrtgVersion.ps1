<#
.SYNOPSIS
Sets the version of all components used when building PrtgAPI

.DESCRIPTION
The Set-PrtgVersion cmdlet updates the version of PrtgAPI. The Set-PrtgVersion cmdlet allows the major, minor, build and revision components to be replaced with any arbitrary version. Typically the Set-PrtgVersion cmdlet is used to revert mistakes made when utilizing the Update-PrtgVersion cmdlet as part of a normal release, or to reset the version when updating the major or minor version components.

.PARAMETER Version
The version to set PrtgAPI to. Must at least include a major and minor version number.

.PARAMETER Legacy
Specifies whether to increase the version used when compiling PrtgAPI using the .NET Core SDK or the legacy .NET Framework tooling.

.EXAMPLE
C:\> Set-PrtgVersion 1.2.3
Set the version to version 1.2.3.0

.EXAMPLE
C:\> Set-PrtgVersion 1.2.3.4
Set the version to version 1.2.3.4. Systems that only utilize the first three version components will be versioned as 1.2.3

.LINK
Get-PrtgVersion
Update-PrtgVersion
#>
function Set-PrtgVersion
{
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true, Position = 0)]
        [Version]$Version,

        [ValidateScript({
            if($_ -and !(Test-IsWindows)) {
                throw "Parameter is only supported on Windows."
            }
            return $true
        })]
        [Parameter(Mandatory = $false)]
        [switch]$Legacy
    )

    $old = Get-PrtgVersion -Legacy:$Legacy -ErrorAction SilentlyContinue

    SetPrtgVersionInternal $Version (-not $Legacy)

    $new = Get-PrtgVersion -Legacy:$Legacy -ErrorAction SilentlyContinue

    $result = [PSCustomObject]@{
        Package = $null
        Assembly = $null
        File = $null
        Module = $null
        ModuleTag = $null
    }

    if($old.PreviousTag)
    {
        $result | Add-Member PreviousTag $null
    }

    foreach($property in $old.PSObject.Properties)
    {
        $result.($property.Name) = ([PrtgVersionChange]::new($old.($property.Name), $new.($property.Name)))
    }

    return $result
}

function SetPrtgVersionInternal($version, $isCore)
{
    SetVersionProps $version $isCore

    SetPsd1Props $version
}

function SetVersionProps($version, $isCore)
{
    $major = $version.Major
    $minor = $version.Minor
    $build = $version.Build
    $revision = $version.Revision

    if($build -eq -1)
    {
        $build = 0
    }

    if($revision -eq -1)
    {
        $revision = 0
    }

    $version = "$major.$minor.$build"
    $assemblyVersion = "$major.$minor.0.0"
    $fileVersion = "$major.$minor.$build.$revision"

    SetVersionPropsCore $version $assemblyVersion $fileVersion
    SetVersionPropsDesktop $version $assemblyVersion $fileVersion
}

function SetVersionPropsCore($version, $assemblyVersion, $fileVersion)
{
    $newContent = @"
<!-- This code was generated by a tool. Any changes made manually will be lost -->
<!-- the next time this code is regenerated. -->

<Project>
  <PropertyGroup>
    <Version>$version</Version>
    <AssemblyVersion>$assemblyVersion</AssemblyVersion>
    <FileVersion>$fileVersion</FileVersion>
  </PropertyGroup>
</Project>
"@

    $root = Get-SolutionRoot

    $props = Join-Path $root "build\Version.props"

    Set-Content $props $newContent
}

function SetVersionPropsDesktop($version, $assemblyVersion, $fileVersion)
{
    $newContent = @"
// This code was generated by a tool. Any changes made manually will be lost
// the next time this code is regenerated.

using System.Reflection;

[assembly: AssemblyVersion("$assemblyVersion")]
[assembly: AssemblyFileVersion("$fileVersion")]
"@

    $root = Get-SolutionRoot

    $props = Join-Path $root "src\PrtgAPI\Properties\Version.cs"

    Set-Content $props $newContent
}

function SetPsd1Props($version)
{
    $root = Get-SolutionRoot
    $psd1Path = Join-Path $root "src\PrtgAPI.PowerShell\PowerShell\Resources\PrtgAPI.psd1"

    if(!(Test-Path $psd1Path))
    {
        throw "Cannot find file '$psd1Path' required for PowerShell Module versioning."
    }

    $psd1Contents = gc $psd1Path

    $major = $version.Major
    $minor = $version.Minor
    $build = $version.Build

    if($build -eq -1)
    {
        $build = 0
    }

    $version = "$major.$minor.$build"

    $newContents = $psd1Contents | foreach {

        if($_ -like "ModuleVersion = '*")
        {
            $newLine = ($_ -replace "ModuleVersion = '(.+?)'","`ModuleVersion = '$version'")

            return $newLine
        }
        elseif($_ -match ".+ReleaseNotes = '.+/tag.+")
        {
            $newLine = $_ -replace "(.+ReleaseNotes = '.+/tag/)(.+)","`$1v$version"

            return $newLine
        }

        $_
    }

    Set-Content $psd1Path $newContents
}

class PrtgVersionChange
{
    [string]$Old
    [string]$New

    PrtgVersionChange($old, $new)
    {
        $this.Old = $old
        $this.New = $new
    }

    [string]ToString()
    {
        if($this.Old -eq $this.New)
        {
            return $this.Old
        }

        return "$($this.Old) -> $($this.New)"
    }
}