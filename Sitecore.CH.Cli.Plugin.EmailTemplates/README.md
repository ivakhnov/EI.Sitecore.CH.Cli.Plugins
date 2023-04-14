# Email templates plugin
This is plugin allows to export/import email templates (i.e. entities of 'M.Mailing.Template' definition) between instances of Content Hub.


The following commands were added to the Content Hub CLI by email-templates plugin:

```
PS C:\code\ch-cli> .\ch-cli.exe email-templates -h
email-templates:
  Commands for import/export of email templates

Usage:
  ch-cli email-templates [options] [command]

Options:
  -?, -h, --help     Show help and usage information
  -v, --verbosity    Enables verbose logging.

Commands:
  export     Exports email templates to disk
  import     Imports email templates from disk
  compare    Compares email templates between disk and endpoint
```

In the code repository there is also Powershell script that executes all export/import commands. By default script executes the deployment from ric.dev endpoint to ric.val endpoint.

```
.\deploy-email-templates.ps1
```

Scripts supports the following parameters:

    SourceEnv - name of source endpoint for export (default ric.dev)
    TargetEnv - name of target endpoint for import (default riv.val)
    Output - folder where email templates will be stored before import
    CliFolder - folder where ch-cli.exe is installed. You don't need to use this parameters if ch-cli.exe was added to the system PATH variable.

```
.\deploy-email-templates.ps1 -SourceEnv "" -TargetEnv ""  -Output "" -CliFolder ""
```
