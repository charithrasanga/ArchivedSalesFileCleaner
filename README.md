
Archived Sales File Cleaner
===========================

Archived Sales File Cleaner is a console application that searches for and deletes or archives sales files older than a specified number of days.

Prerequisites
-------------

-   .NET 6 or higher.
-   Visual Studio or VSCode


Configuration
-------------

The application can be configured by modifying the `appsettings.json` file. The following options are available:

### Delete Settings

-   `RetentionPeriodInDays`: The number of days after which sales files should be deleted or archived.
-   `SourceDirectory`: The directory in which sales files are stored.
-   `RemoveFilePhysically`: If set to true, sales files will be permanently deleted. If set to false, they will be archived to the directory specified by ArchivePath.
-   `ArchivePath`: The directory to which sales files should be archived.

### Serilog Configuration

Serilog is a popular logging library for .NET that provides a flexible and extensible platform for logging. The library allows logging to a variety of targets, including the console, files, databases, and third-party services like Seq and Elasticsearch.

Serilog configuration is done through an `appsettings.json` file, which is a configuration file that stores key-value pairs for different settings. The Serilog section of the `appsettings.json` file contains the settings required for configuring Serilog.

#### Sample Configuration
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.txt",
          "rollingInterval": "Day",
          "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
        }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
    "Properties": {
      "Application": "ArchivedSalesFileCleaner"
    }
  }
}
```

#### Configuration Options

-   `MinimumLevel`: The minimum log level that will be written. This setting can be overridden for specific namespaces.
-   `WriteTo`: The list of sinks that the logs will be written to. Serilog provides several built-in sinks, such as the console sink and the file sink. Each sink can have its own configuration options.
-   `Enrich`: The list of enrichers that add additional information to the log events. Serilog provides several built-in enrichers, such as the `FromLogContext` enricher, which adds properties from the log context to the log event.
-   `Properties`: Additional properties that will be added to all log events.

#### Icon 
Icon was downloaded from [https://www.iconfinder.com](https://www.iconfinder.com/icons/2462966/delete_eliminate_garbage_litter_recycle_remove_trash_icon)