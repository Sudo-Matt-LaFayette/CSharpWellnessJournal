# Daily Check-In Journal - Architecture & Design Decisions

## Overview

This project is a comprehensive daily check-in and journaling system designed specifically for individuals with autism and ADHD. The application helps users track their mental health, identify patterns, and make data-driven decisions about self-care strategies.

## My Approach: Bottom-Up Design Philosophy

When I approach a project, I start with the foundational building blocks and work my way up. This bottom-up methodology ensures that each layer is solid before building on top of it. For this project, I began by defining the core data models, then built the services that manipulate that data, followed by the view models that coordinate business logic, and finally the views that present the information to users.

This approach has several advantages:
- **Testability**: Each layer can be tested independently
- **Maintainability**: Changes to one layer don't cascade unpredictably
- **Clarity**: The dependencies flow in one direction, making the codebase easier to understand
- **Flexibility**: Individual components can be swapped out without affecting the entire system

## Project Structure

### Data Layer (Models)

I started here because data is the foundation of everything. The models represent the core entities:

- **CheckIn.cs**: The central entity representing a day's worth of data. I separated Morning and Evening check-ins as separate objects within a single CheckIn to maintain clear temporal boundaries while keeping related data together.

- **Goal.cs**: Tracks both short-term (weekly) and long-term (monthly/yearly) goals. I included a progress entry system to allow for granular tracking over time.

- **Habit.cs**: Designed with extensibility in mind. The `IsCustom` flag allows the system to distinguish between default habits and user-created ones, which is useful for analytics and UI presentation.

- **Pattern.cs**: This was interesting to design. Patterns are detected automatically, but I wanted them to be persistent entities that could be tracked over time. The `Data` dictionary allows for flexible pattern-specific information without creating a rigid schema.

- **AppSettings.cs**: Centralized configuration. I chose to store settings as a single object rather than individual files to ensure atomic updates and easier serialization.

**Design Decision**: I used nullable reference types throughout (`string?`, `CheckIn?`) to make the code's intent explicit about what can and cannot be null. This reduces runtime errors and makes the code self-documenting.

### Service Layer

Services encapsulate business logic and data access. I designed them as interfaces first, then implementations, following the Dependency Inversion Principle.

**IDataService / DataService.cs**:
- Handles all persistence operations
- Uses JSON for simplicity and human-readability (important for a personal journaling app where users might want to inspect their data)
- Stores data in `%LocalAppData%` following Windows conventions
- I chose async/await throughout because file I/O operations should never block the UI thread
- Comprehensive logging of all file operations (loads, saves, errors) to aid in debugging data persistence issues

**IPatternDetectionService / PatternDetectionService.cs**:
- This is where the analytical aspect of the project really shines
- I implemented pattern detection algorithms that analyze historical data
- Each pattern type (energy, mood, sleep, etc.) has its own detection logic
- The service compares new patterns against existing ones to avoid duplicates
- Patterns are only detected after sufficient data (7+ days) to ensure statistical significance

**INotificationService / NotificationService.cs**:
- Handles both Windows toast notifications and in-app notifications
- Uses a timer-based scheduling system that recalculates daily
- I implemented a fallback to MessageBox if toast notifications fail, ensuring the user always gets notified

**IThemeService / ThemeService.cs**:
- Manages theme switching at runtime
- Themes are stored as separate ResourceDictionary files
- The service updates the application's merged dictionaries dynamically

**IGoogleDriveService / GoogleDriveService.cs**:
- Implements OAuth 2.0 authentication flow
- Designed with extensibility in mind - the actual sync logic is stubbed but the architecture supports it
- Stores tokens securely in the settings

**ILoggerService / LoggerService.cs**:
- Comprehensive logging system using Serilog
- Logs to both console (for development) and file (for debugging)
- Log files are stored in `%LocalAppData%\DailyCheckInJournal\Logs\` with daily rotation
- Maintains 30 days of log history
- Logs at multiple levels: Debug, Information, Warning, Error
- Critical for debugging crashes and understanding application behavior
- All services and ViewModels can inject ILoggerService for logging

**Design Decision**: All services are registered as singletons in the dependency injection container. This ensures state consistency (like notification timers) and efficient resource usage. The LoggerService is registered first so it's available to all other services during initialization.

### ViewModel Layer

ViewModels coordinate between the views and services. I used the MVVM pattern because:

1. **Separation of Concerns**: Business logic is separate from UI code
2. **Testability**: ViewModels can be unit tested without UI
3. **Data Binding**: WPF's powerful data binding system works seamlessly with ViewModels
4. **Reusability**: The same ViewModel could theoretically power different UI implementations

**MainViewModel.cs**:
- Acts as the navigation coordinator
- Holds references to all child ViewModels
- Initializes reminder scheduling on startup
- Uses commands for navigation (following the Command pattern)
- Logs navigation events and initialization steps for debugging

**MorningCheckInViewModel.cs / EveningCheckInViewModel.cs**:
- Load existing check-in data for the current day on initialization
- Use ObservableCollections for all lists to enable automatic UI updates
- Separate save operations for morning and evening to allow partial completion
- I load habits dynamically from the data service to ensure the UI always reflects the current habit list

**WeeklyReviewViewModel.cs**:
- Implements date range selection with automatic recalculation
- Generates reports on-demand (not automatically) to give users control
- Calculates statistics using LINQ for clean, readable code
- Integrates with pattern detection to show insights

**VisualizationViewModel.cs**:
- Uses LiveCharts library for charting (chosen for its modern API and good WPF integration)
- Series are stored as ObservableCollections so charts update automatically
- Date range filtering allows users to focus on specific time periods

**SettingsViewModel.cs**:
- Manages all application configuration
- Provides commands for adding/removing habits and mistake categories
- Integrates with Google Drive service for authentication
- Saves settings and immediately applies changes (like theme switching)

**CalendarViewModel.cs**:
- Manages calendar display and date selection
- Loads all check-in dates to highlight days with entries
- Provides date selection functionality with automatic check-in loading
- Emits events when users request to view full day details
- Integrates with MainViewModel for navigation to DayView

**DayViewViewModel.cs**:
- Displays a selected day's check-in in read-only mode
- Separates morning and evening check-in data for clear presentation
- Loads all check-in details including medications, habits, triggers, coping strategies, etc.
- Uses ObservableCollections for dynamic UI updates
- Designed for viewing historical data without editing capabilities

**Design Decision**: I used CommunityToolkit.Mvvm for the ViewModels. This library provides:
- `ObservableObject` base class with automatic property change notifications
- `RelayCommand` and `AsyncRelayCommand` for command implementation
- Reduces boilerplate code significantly

### View Layer

Views are pure XAML with minimal code-behind. I kept code-behind to just `InitializeComponent()` calls, following MVVM best practices.

**DataTemplates in App.xaml**:
- I used DataTemplates to automatically resolve ViewModels to Views
- This eliminates the need for manual view creation in code
- Makes the navigation system cleaner and more maintainable

**XAML Structure**:
- Used GroupBoxes extensively to organize related fields
- Sliders for numeric inputs (1-10 scales) provide visual feedback
- ScrollViewers wrap all views to handle content overflow gracefully
- Consistent spacing using Margin properties (I considered using a Spacing property but stuck with Margin for broader compatibility)

**CalendarView.xaml**:
- Uses WPF's built-in Calendar control for date selection
- Displays a preview of selected day's check-in summary
- Shows morning and evening check-in highlights
- Provides "View Full Details" button to navigate to complete day view
- Refresh button to reload check-in dates

**DayView.xaml**:
- Read-only view for displaying complete check-in details
- Separates morning and evening sections for clarity
- Uses TextBlocks instead of TextBoxes to prevent editing
- Displays all check-in data including medications, habits, triggers, coping strategies, etc.
- Uses CollectionToVisibilityConverter to show/hide sections based on data availability

**Design Decision**: I removed the `PlaceholderText` attributes from TextBoxes because WPF doesn't natively support placeholders. While I could have created a custom control, I prioritized getting the core functionality working first. This is a good candidate for future enhancement.

### Converters

I created several value converters to handle data transformation between ViewModels and Views:

- **ListToStringConverter**: Converts lists to comma-separated strings (for sensory triggers, tags, etc.)
- **NullToVisibilityConverter**: Hides UI elements when data is null
- **BoolToRadioConverter**: Handles three-state radio buttons (Yes/No/N/A) for nullable booleans
- **NullToRadioConverter**: Specifically for the N/A option in radio button groups
- **DateTimeToDateConverter**: Converts DateTime to Date for DatePicker controls
- **NullToBoolConverter**: Converts null values to boolean for enabling/disabling UI elements
- **CollectionToVisibilityConverter**: Shows/hides UI elements based on whether collections have items

**Design Decision**: Converters are registered in App.xaml as resources, making them available application-wide. This follows the DRY principle.

### Controls

**TimePicker.xaml / TimePicker.xaml.cs**:
- Custom user control for time selection
- Uses ComboBoxes for hours and minutes
- Implements a DependencyProperty for two-way data binding
- Minutes are incremented by 5 to reduce complexity while maintaining usefulness

### Themes

I created 6 themes to accommodate different user preferences and accessibility needs:

- **Default**: Balanced, professional appearance
- **Dark**: Reduces eye strain for extended use
- **Light**: High contrast, clean appearance
- **High Contrast**: Accessibility-focused with strong borders
- **Autumn**: Warm, calming colors
- **Ocean**: Cool, soothing palette

**Design Decision**: Themes are stored as separate ResourceDictionary files. This makes it easy to add new themes without modifying existing code. The ThemeService dynamically loads and applies themes at runtime.

## Dependency Injection

I used Microsoft.Extensions.DependencyInjection for several reasons:

1. **Loose Coupling**: Components don't directly instantiate their dependencies
2. **Testability**: Dependencies can be easily mocked for unit testing
3. **Lifetime Management**: Services are registered with appropriate lifetimes (singleton vs transient)
4. **Industry Standard**: This is the same DI container used in ASP.NET Core, making the skills transferable

The service registration happens in `App.xaml.cs` in the `ConfigureServices` method, keeping all dependency configuration in one place. The LoggerService is initialized first and registered as a singleton, ensuring it's available to all other services during their construction.

## Data Persistence

I chose JSON for data storage because:

1. **Human-Readable**: Users can inspect and backup their data easily
2. **No External Dependencies**: No database server required
3. **Simple Serialization**: Newtonsoft.Json handles complex object graphs well
4. **Portable**: JSON files can be easily moved between machines

Data is stored in `%LocalAppData%\DailyCheckInJournal\` following Windows application data conventions. Each entity type has its own file, which allows for efficient partial loading if needed in the future.

## Pattern Detection Algorithm

The pattern detection system is one of the more interesting parts of this project. Here's my approach:

1. **Energy Patterns**: Analyzes day-of-week patterns in energy levels. Only flags patterns if there's a significant difference (≥2 points) to avoid false positives.

2. **Overcommitment Patterns**: Tracks which days of the week tend to have overcommitment issues. This helps users plan their schedules better.

3. **Mistake Patterns**: Groups mistakes by category and identifies frequently occurring ones. The threshold is 3+ occurrences in 30 days to balance sensitivity with usefulness.

4. **Mood Patterns**: Calculates average mood over a 2-week period. Flags if average drops below 4/10, suggesting potential issues.

5. **Sleep Patterns**: Similar to mood, tracks sleep quality trends. Poor sleep (≤5/10 average) is flagged as it significantly impacts daily functioning.

**Design Decision**: Patterns are stored persistently, not just calculated on-demand. This allows the system to track pattern evolution over time and avoid re-notifying about the same pattern repeatedly.

## Error Handling Strategy

I implemented a defensive programming approach:

- **Null Checks**: Extensive use of null-conditional operators (`?.`) and null-coalescing (`??`)
- **Try-Catch Blocks**: Around operations that could fail (file I/O, notifications)
- **Graceful Degradation**: If toast notifications fail, fall back to MessageBox
- **Default Values**: Models initialize with sensible defaults to prevent null reference exceptions
- **Comprehensive Logging**: All exceptions are logged with full stack traces to aid in debugging
- **Global Exception Handlers**: App.xaml.cs includes handlers for both AppDomain and Dispatcher unhandled exceptions
- **User-Friendly Error Messages**: Errors are logged with technical details, but users see simplified messages

## Logging System

I implemented a comprehensive logging system using Serilog because:

1. **Debugging**: When the application crashes, logs provide detailed information about what went wrong
2. **Troubleshooting**: Logs help identify issues in production without requiring a debugger
3. **Audit Trail**: Important operations (data saves, pattern detection) are logged for traceability
4. **Development**: Console logging provides immediate feedback during development

**Implementation Details**:
- Logs are written to both console (for immediate feedback) and files (for persistence)
- Log files are stored in `%LocalAppData%\DailyCheckInJournal\Logs\`
- Files are rotated daily (one file per day) and retained for 30 days
- Log levels: Debug (detailed), Information (general), Warning (non-critical issues), Error (exceptions)
- All services and ViewModels can inject ILoggerService for logging
- Global exception handlers ensure all unhandled exceptions are logged

**Design Decision**: I chose Serilog over other logging frameworks because:
- It's widely used in the .NET ecosystem
- Excellent file and console sinks
- Structured logging capabilities (though not fully utilized yet)
- Easy to configure and extend
- Good performance

The logging system was added after initial development to help debug crashes. This demonstrates my ability to add infrastructure features to an existing codebase without disrupting the architecture.

## Future Enhancements (Not Yet Implemented)

While the core functionality is complete, there are several areas I'd like to enhance:

1. **Password Protection**: The framework is in place, but encryption logic needs to be implemented
2. **Google Drive Sync**: Authentication is implemented, but file upload/download logic needs completion
3. **Data Export**: CSV/PDF export would be useful for external analysis
4. **Advanced Visualizations**: Correlation charts, heatmaps, trend analysis
5. **Reminder Customization**: Allow different reminder times for different days of the week
6. **Data Validation**: More robust input validation and error messages
7. **Unit Tests**: Comprehensive test coverage for services and ViewModels

## Technical Choices & Trade-offs

**WPF vs WinUI 3**: I chose WPF because:
- More mature ecosystem
- Better third-party library support (LiveCharts, etc.)
- Familiar to most .NET developers
- Runs on .NET 8, which provides good performance

**MVVM vs Code-Behind**: MVVM provides better separation of concerns and testability, even if it requires more initial setup.

**JSON vs SQLite**: JSON is simpler for this use case. SQLite would add complexity without significant benefits for a single-user application. However, if this were multi-user or needed complex queries, SQLite would be the better choice.

**LiveCharts vs Other Charting Libraries**: LiveCharts has a modern API, good performance, and active development. It's also free and open-source.

## Code Quality Practices

Throughout the project, I've followed several principles:

1. **SOLID Principles**: Especially Single Responsibility and Dependency Inversion
2. **DRY (Don't Repeat Yourself)**: Converters, services, and reusable components
3. **Explicit Naming**: Variables and methods clearly describe their purpose
4. **Consistent Formatting**: Following C# coding conventions
5. **Comments**: Added where logic is non-obvious or where design decisions need explanation

## Conclusion

This project demonstrates my ability to:
- Design a system from the ground up using a bottom-up approach
- Make thoughtful architectural decisions with clear reasoning
- Implement complex features (pattern detection, data visualization)
- Write maintainable, testable code following industry best practices
- Balance feature completeness with code quality

The codebase is structured to be easily extensible. Adding new check-in fields, pattern types, or visualization types follows clear patterns established in the existing code.

