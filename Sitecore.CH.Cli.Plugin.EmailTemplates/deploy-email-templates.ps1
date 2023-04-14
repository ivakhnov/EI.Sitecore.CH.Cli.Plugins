[CmdletBinding(SupportsShouldProcess)]
param (
    [string]
    $SourceEnv = "dev",
    [string]
    $TargetEnv = "val",
    [string]
    # Folder where email templates will be exported or from where will be imported
    $Output,
    [string]
    # Folder where Content Hub CLI is installed
    $CliFolder
)
<#
    # you need to set endpoints before you will run this command
    &$cliPath endpoint add -n $name -r $url -i $clientId -s $clientSecret -u $username -p $password

    .\deploy-email-templates.ps1 -Output "" -CliFolder ""


    # How to setup path to ch-cli permamently - NOTE: you must run powershell as administrator 
    [Environment]::SetEnvironmentVariable("PATH", $Env:PATH + ";C:\code\ch-cli", [EnvironmentVariableTarget]::Machine)
#>

# Create output folder if not exists
if( [string]::IsNullOrEmpty($Output) )
{
    $Output = Join-Path $PSScriptRoot -ChildPath "email-templates-$SourceEnv"
    New-Item $Output -ItemType Directory -Force
}

# Set path to ch-cli executable
$cliPath = "ch-cli.exe"
if( ![string]::IsNullOrEmpty($CliFolder) )
{
    $cliPath = Join-Path $CliFolder -ChildPath "ch-cli.exe"
}


# export email templates from source environment
&$cliPath endpoint select -n $SourceEnv
&$cliPath email-templates export -o $Output

# import email templates to target environment
&$cliPath endpoint select -n $TargetEnv

if ($PSCmdlet.ShouldProcess($TargetEnv)) {
    &$cliPath email-templates import -s $Output
} else {
    &$cliPath email-templates import -s $Output --whatif
}
