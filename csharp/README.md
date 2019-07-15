# CSharp code examples

This directory contains executable examples of how to use https://api.aweber.com/ from within .NET application using C#. The examples are built with .NET Core and will require version 2.2.0 or greater to be installed. You can download .NET Core [here](https://dotnet.microsoft.com/download). 

If you haven't read [CONTRIBUTING], please take a moment to read it now.  It describes the basic configuration and process and this document picks up where it leaves off.

## Quickstart
These examples use NuGet for dependency management. NuGet is available by default when using Visual Studio (recommended). 

## Installing requirements
The requirements are in each examples *.csproj* file. Installation will happen automatically (default) when you build in Visual Studio. If automatic NuGet package restore is not enabled, please Run package restore for the entire solution.
    
## Running examples
You will need to create a *credentials.json* file using your existing client ID and secret from the
[My Apps page](https://labs.aweber.com/apps) and running the GetAccessToken example console application.
    
The rest of the examples will use this file for authentication. 

## Refreshing tokens
If your access token is expired, it will be refreshed automatically when you run the next example.

[CONTRIBUTING]: https://github.com/aweber/public-api-examples/blob/master/CONTRIBUTING.md