# Content Hub command line interface (CLI) - Plugins development

This project is intended to help in the development and aggregation of custom plugins for the Content Hub CLI. It provides examples that can be used as a starting point for development of additional plugins.

A plugin is a library that extends the CLI with additional commands. With the CLI, you can manage content and media from a Content Hub instance. The CLI uses the SDK to communicate with Content Hub instances through the API, based on commands in the form of lines of text. 

Before you start you need to install ch-cli and configure endpoints as described in the [Sitecore documentation](https://doc.sitecore.com/ch/en/developers/42/cloud-dev/integration-tools--content-hub-cli.html).

Each folder holds a separate plugin inplementation and contains its own dedicated README.