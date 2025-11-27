# Logging System

## Overview

The application uses Serilog for comprehensive logging. All logs are written to both the console (for immediate feedback) and log files (for debugging).

## Log File Location

Logs are stored in:
```
%LocalAppData%\DailyCheckInJournal\Logs\
```

On Windows, this is typically:
```
C:\Users\[YourUsername]\AppData\Local\DailyCheckInJournal\Logs\
```

## Log Files

- **Format**: `app-YYYYMMDD.log` (one file per day)
- **Retention**: 30 days (older logs are automatically deleted)
- **Format**: Timestamp, Log Level, Message, Exception details

## Log Levels

The application uses the following log levels:

- **Debug**: Detailed information for debugging (service initialization, method entry/exit)
- **Information**: General informational messages (application startup, successful operations)
- **Warning**: Warning messages (non-critical issues)
- **Error**: Error messages (exceptions, failures)

## What Gets Logged

### Application Startup
- Service initialization
- Dependency injection configuration
- Window creation
- Reminder scheduling

### Data Operations
- Loading check-ins, goals, habits, patterns, settings
- Saving data
- File I/O operations
- Errors during data operations

### Navigation
- View changes
- Command execution

### Errors
- All unhandled exceptions
- Service initialization failures
- Data access errors
- UI errors

## Viewing Logs

### During Development
When running `dotnet run`, logs appear in the console output.

### After a Crash
1. Navigate to `%LocalAppData%\DailyCheckInJournal\Logs\`
2. Open the most recent log file (sorted by date)
3. Look for entries with `[ERR]` (errors) or `[WRN]` (warnings)
4. Check the stack traces for exception details

### Log File Format Example
```
2024-01-15 10:30:45.123 -05:00 [INF] Application starting...
2024-01-15 10:30:45.234 -05:00 [DBG] Configuring services...
2024-01-15 10:30:45.345 -05:00 [DBG] Initializing DataService...
2024-01-15 10:30:45.456 -05:00 [INF] DataService initialized successfully
2024-01-15 10:30:45.567 -05:00 [ERR] Error initializing MainViewModel
System.NullReferenceException: Object reference not set to an instance of an object.
   at DailyCheckInJournal.ViewModels.MainViewModel..ctor(...)
   ...
```

## Debugging Tips

1. **Check the most recent log file** - Errors are usually at the end
2. **Look for stack traces** - They show exactly where the error occurred
3. **Check timestamps** - See the sequence of events leading to the error
4. **Search for "Error" or "Exception"** - Quick way to find problems

## Common Error Patterns

### NullReferenceException
- Usually indicates a missing dependency or uninitialized object
- Check the stack trace to see which object was null

### FileNotFoundException
- Data files might be missing or in wrong location
- Check that `%LocalAppData%\DailyCheckInJournal\` exists

### JsonException
- Corrupted JSON files
- Check the data files in `%LocalAppData%\DailyCheckInJournal\`

### Dependency Injection Errors
- Usually happens during startup
- Check that all services are registered correctly in `App.xaml.cs`

## Adding More Logging

To add logging to a new service or ViewModel:

1. Inject `ILoggerService` in the constructor
2. Use logging methods:
   ```csharp
   _logger?.LogDebug("Detailed debug information");
   _logger?.LogInformation("General information");
   _logger?.LogWarning("Warning message");
   _logger?.LogError(exception, "Error message with exception");
   ```

## Logging Best Practices

- **Log at appropriate levels**: Debug for detailed info, Information for important events, Error for exceptions
- **Include context**: Log what operation was being performed when the error occurred
- **Log exceptions**: Always log exceptions with full details
- **Don't log sensitive data**: Avoid logging passwords, tokens, or personal information

