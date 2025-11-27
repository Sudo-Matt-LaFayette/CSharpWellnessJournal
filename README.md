# Daily Check-In Journal

A comprehensive daily check-in and journaling system designed to help individuals with autism and ADHD track their mental health, identify patterns, and make data-driven decisions about self-care strategies.

## Features

- **Morning & Evening Check-Ins**: Track energy levels, mood, sleep, habits, and more
- **Weekly Reviews**: Comprehensive statistics and pattern analysis
- **Data Visualizations**: Charts and graphs to visualize trends over time
- **Pattern Detection**: Automatically identifies patterns in your data
- **Notifications**: Reminders for morning and evening check-ins
- **Customizable Themes**: Six themes including accessibility options
- **Comprehensive Logging**: Full logging system for debugging and troubleshooting

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Windows 10 or later

### Running the Application

```bash
cd "C:\Users\lafay\Desktop\Projects\Vibe Coding Projects\CSharpWellnessJournal"
dotnet restore
dotnet run
```

Or open the project in Visual Studio and press F5.

## Documentation

- **[ARCHITECTURE.md](ARCHITECTURE.md)**: Detailed architecture and design decisions
- **[DESIGN_RATIONALE.md](DESIGN_RATIONALE.md)**: Explanation of why the project is built this way
- **[PROJECT_OVERVIEW.md](PROJECT_OVERVIEW.md)**: High-level project overview
- **[TESTING_GUIDE.md](TESTING_GUIDE.md)**: Guide for testing the application
- **[LOGGING.md](LOGGING.md)**: Information about the logging system

## Development Workflow Practice

This project is also used to practice professional development workflows:

- **[START_HERE.md](START_HERE.md)**: 🚀 **Begin here!** Step-by-step guide for your first workflow cycle
- **[DEVELOPMENT_WORKFLOW_GUIDE.md](DEVELOPMENT_WORKFLOW_GUIDE.md)**: Complete guide to development workflows (PRs, code reviews, etc.)
- **[WORKFLOW_CHECKLIST.md](WORKFLOW_CHECKLIST.md)**: Checklist to follow for each story
- **[TIME_TRACKING.md](TIME_TRACKING.md)**: Track time spent on tasks vs. estimates

## Data Storage

Your data is stored locally in:
```
%LocalAppData%\DailyCheckInJournal\
```

Logs are stored in:
```
%LocalAppData%\DailyCheckInJournal\Logs\
```

## Technologies

- .NET 8.0
- WPF (Windows Presentation Foundation)
- MVVM Pattern
- Dependency Injection
- Serilog (Logging)
- LiveCharts (Data Visualization)
- Newtonsoft.Json (Data Serialization)

## Project Structure

```
DailyCheckInJournal/
├── Models/              # Data structures
├── Services/            # Business logic and data access
├── ViewModels/          # Presentation logic
├── Views/               # UI (XAML)
├── Converters/          # Value converters for data binding
├── Controls/            # Custom user controls
└── Themes/              # Visual styling resources
```

## Troubleshooting

If the application crashes or encounters errors:

1. Check the log files in `%LocalAppData%\DailyCheckInJournal\Logs\`
2. Look for `[ERR]` entries in the most recent log file
3. Review the stack traces to identify the issue
4. See [LOGGING.md](LOGGING.md) for more information about the logging system

## License

This project is for personal use.

