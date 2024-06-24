# Package cleaner plugin
This plugin does not connect to any ContentHub environment, but rather allows to clean packages that are supposed to be moved between different ContentHub environments such as DEV, QA & PROD. The purpose of cleaning is to allow to addition of those packages into a version control system repository. This way, there a no redundant and confusing "diffs" between files that are essentially the same.
This is plugin allows to export/import email templates (i.e. entities of 'M.Mailing.Template' definition) between instances of Content Hub.


The following commands were added to the Content Hub CLI by the import package cleaner plugin:

```
PS C:\code\ch-cli> .\ch-cli.exe email-templates -h
import-package-cleaner:
  Commands for cleaning the import package

Usage:
  ch-cli import-package-cleaner [options] [command]

Options:
  -?, -h, --help     Show help and usage information
  -v, --verbosity    Enables verbose logging.

Commands:
  clean     Cleans the import package on the disk, make it ready to be added to a version control system repository.
```

Scripts supports the following parameters:

    ShouldCleanPortalComponents - If Portal Components should be cleaned from Portal Pages in related section. Default is FALSE.
    ShouldCleanActionVariables - If should clean the variable details in M.Action entities (e.g. apiUrl, headers, connectionString). Default is FALSE.

