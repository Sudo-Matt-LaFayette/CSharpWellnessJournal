# Daily Check-In Journal - Project Overview

## Purpose

This application is a comprehensive daily check-in and journaling system designed to help individuals with autism and ADHD track their mental health, identify patterns, and make data-driven decisions about self-care strategies.

## My Development Process

I approach software development with a bottom-up methodology. Rather than starting with the UI and working backwards, I begin with the foundational data structures and build upward:

1. **Data Models** - Define what information the system needs to capture
2. **Services** - Implement business logic and data persistence
3. **ViewModels** - Coordinate between UI and services
4. **Views** - Present information to users

This approach ensures each layer is solid before building on top of it, resulting in a more maintainable and testable codebase.

## Key Features

### Morning Check-In
- Energy level assessment (1-10 scale)
- Must-do task identification
- Capacity feeling evaluation
- Sleep quality tracking from previous night
- Medication logging
- Sensory state monitoring
- Executive function assessment
- Emotional state tracking
- Habit tracking
- Gratitude/positives logging
- Trigger identification
- Coping strategy documentation

### Evening Check-In
- Must-do completion status
- Energy level reassessment
- Overcommitment tracking
- Common mistakes logging (with categories, tags, and notes)
- Emotional state review
- Habit completion tracking
- Gratitude reflection
- Trigger and coping strategy review
- Additional notes

### Weekly Review
- Comprehensive statistics (average energy, mood, completion rates)
- Automatic pattern detection
- Weekly summary reports with structured prompts
- Daily check-in overview

### Visualizations
- Energy level trends over time
- Mood patterns
- Sleep quality charts
- Mistake frequency analysis
- Active pattern displays

### Additional Features
- **Pattern Detection**: Automatically identifies patterns in energy, mood, sleep, mistakes, and overcommitment
- **Notifications**: System notifications and in-app reminders for morning and evening check-ins
- **Themes**: Six customizable themes (Default, Dark, Light, High Contrast, Autumn, Ocean)
- **Google Drive Sync**: Optional cloud backup and synchronization
- **Privacy & Security**: Password protection framework
- **Customizable Habits**: Add and manage personal habits
- **Mistake Categories**: Customize mistake categories for better tracking
- **Comprehensive Logging**: Full logging system using Serilog for debugging and troubleshooting

## Technical Architecture

### Design Patterns
- **MVVM (Model-View-ViewModel)**: Separates business logic from UI
- **Dependency Injection**: Loose coupling between components
- **Repository Pattern**: Abstracted data access through IDataService
- **Command Pattern**: User actions handled through ICommand implementations

### Technologies
- **.NET 8.0**: Modern C# runtime with excellent performance
- **WPF**: Rich desktop UI framework
- **CommunityToolkit.Mvvm**: Reduces MVVM boilerplate
- **LiveCharts**: Modern charting library
- **Newtonsoft.Json**: JSON serialization
- **Microsoft.Extensions.DependencyInjection**: Dependency injection container
- **Serilog**: Comprehensive logging framework with file and console sinks

### Project Structure
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

## Design Decisions

### Why MVVM?
MVVM provides clear separation of concerns, making the codebase more testable and maintainable. ViewModels can be unit tested without UI, and the same ViewModel could theoretically power different UI implementations.

### Why Dependency Injection?
Dependency injection enables loose coupling between components. Services don't know about their consumers, and consumers don't know about service implementations. This makes the code more flexible and testable.

### Why JSON Storage?
JSON is human-readable, making it easy for users to inspect and backup their data. It requires no external dependencies (like a database server) and is simple to serialize/deserialize. For a single-user desktop application, JSON provides the right balance of simplicity and functionality.

### Why WPF?
WPF offers powerful data binding, rich UI capabilities, and a mature ecosystem. It's well-suited for desktop applications that need complex, data-driven interfaces.

## Pattern Detection Algorithm

The pattern detection system analyzes historical data to identify meaningful trends:

- **Energy Patterns**: Detects day-of-week patterns in energy levels
- **Overcommitment Patterns**: Identifies which days tend to have overcommitment issues
- **Mistake Patterns**: Tracks frequently occurring mistakes by category
- **Mood Patterns**: Flags extended periods of low mood
- **Sleep Patterns**: Identifies poor sleep quality trends

Patterns are only detected after sufficient data (7+ days) to ensure statistical significance, and thresholds are set to balance sensitivity with usefulness.

## Code Quality

The codebase follows several principles:
- **SOLID Principles**: Especially Single Responsibility and Dependency Inversion
- **DRY (Don't Repeat Yourself)**: Reusable components and services
- **Explicit Naming**: Clear, descriptive variable and method names
- **Consistent Formatting**: Following C# coding conventions
- **Defensive Programming**: Extensive error handling and null checks
- **Comprehensive Logging**: All operations and errors are logged for debugging and troubleshooting

## Logging System

The application includes a comprehensive logging system using Serilog:
- **Log Location**: `%LocalAppData%\DailyCheckInJournal\Logs\`
- **Log Rotation**: Daily rotation with 30-day retention
- **Log Levels**: Debug, Information, Warning, Error
- **Output**: Both console (for development) and file (for persistence)
- **Global Exception Handling**: All unhandled exceptions are logged with full stack traces

This logging system is essential for debugging crashes and understanding application behavior. See `LOGGING.md` for detailed information about the logging system.

## Future Enhancements

While the core functionality is complete, potential future enhancements include:
- Complete password protection with encryption
- Full Google Drive sync implementation
- Data export (CSV/PDF)
- Advanced visualizations (correlation charts, heatmaps)
- Unit test coverage
- More sophisticated pattern detection algorithms

## Conclusion

This project demonstrates my ability to design and implement a complex application from the ground up, making thoughtful architectural decisions at each step. The bottom-up approach ensures a solid foundation, while modern patterns and practices ensure maintainability and extensibility.

