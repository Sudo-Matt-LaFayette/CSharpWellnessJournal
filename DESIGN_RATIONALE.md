# Design Rationale: Why I Built It This Way

## Introduction

When I set out to build this daily check-in journal, I had specific goals in mind: create a tool that would genuinely help me (and others with autism/ADHD) understand patterns in our daily experiences, and build it in a way that demonstrates solid software engineering principles. This document explains the "why" behind the "what" - the reasoning that drove each architectural decision.

## Starting from the Foundation: Data Models

I always begin with data structures because they define what the application can and cannot do. Before writing a single line of UI code, I spent time thinking about:

- What information needs to be captured?
- How does that information relate to other information?
- What questions will users want to answer with this data?

This led me to design models that are:
- **Comprehensive**: Cover all aspects of daily experience (energy, mood, sensory state, executive function, etc.)
- **Flexible**: Use nullable types and optional fields so users aren't forced to fill everything
- **Extensible**: The Pattern model uses a Dictionary for data, allowing new pattern types without schema changes

The separation of MorningCheckIn and EveningCheckIn as nested objects within CheckIn was intentional. It maintains temporal clarity (morning vs evening are distinct time periods) while keeping related data together (both are part of the same day).

## Service Layer: Encapsulating Business Logic

I'm a firm believer in separation of concerns. The service layer exists because:

1. **Testability**: I can test business logic without UI
2. **Reusability**: The same services could power a web app, mobile app, or API
3. **Maintainability**: When business rules change, I only update one place

The PatternDetectionService is particularly interesting. I implemented it as a series of small, focused methods (DetectEnergyPattern, DetectMistakePattern, etc.) rather than one monolithic method. This makes it:
- Easy to understand what each pattern type does
- Simple to add new pattern types
- Testable in isolation

Each detection method follows a similar pattern:
1. Filter/transform the data
2. Calculate statistics
3. Check thresholds
4. Create pattern objects if criteria are met

This consistency makes the code predictable and maintainable.

## MVVM: Not Just a Pattern, a Philosophy

I chose MVVM not because it's trendy, but because it solves real problems:

**Problem**: UI code and business logic get tangled together, making both hard to test and maintain.

**Solution**: ViewModels act as intermediaries. They:
- Hold the state that the view needs
- Execute commands in response to user actions
- Coordinate with services to load/save data
- Transform data for display (though I prefer converters for complex transformations)

The ViewModels use ObservableObject from CommunityToolkit.Mvvm, which automatically implements INotifyPropertyChanged. This eliminates a lot of boilerplate while maintaining the benefits of the pattern.

**Data Binding**: WPF's data binding is powerful, but it requires properties to notify when they change. ObservableObject handles this automatically. When I set a property, the UI updates. No manual event raising, no forgetting to notify - it just works.

## Dependency Injection: Loose Coupling in Practice

I use dependency injection because I've seen what happens without it. In previous projects, I've had classes that directly instantiate their dependencies, creating tight coupling. When I needed to test or swap implementations, it was painful.

With DI:
- MainViewModel doesn't know how to create a DataService - it just receives one
- If I want to test MainViewModel, I can inject a mock DataService
- If I want to change from JSON storage to a database, I just swap the implementation

The service registration in App.xaml.cs is explicit and centralized. Anyone reading the code can immediately see:
- What services exist
- How they're configured
- What their lifetimes are

## Why JSON for Storage?

I considered several options:
- **SQLite**: Overkill for a single-user app. Adds complexity without clear benefits.
- **XML**: More verbose than JSON, harder to read.
- **Binary**: Fast but not human-readable. Users can't inspect or backup their data easily.
- **JSON**: Human-readable, simple, widely supported.

For a personal journaling app, human-readability is important. Users should be able to:
- Open their data file and see what's there
- Make backups easily
- Potentially migrate to another tool if needed

JSON fits these requirements perfectly. Newtonsoft.Json handles serialization/deserialization of complex object graphs, including nested objects and collections.

## Pattern Detection: Making Data Useful

The pattern detection system transforms raw data into actionable insights. Here's my thinking:

**Energy Patterns**: I look for day-of-week patterns because many people have different energy levels on weekdays vs weekends, or on specific days. The algorithm:
1. Groups energy levels by day of week
2. Calculates averages
3. Finds the highest and lowest days
4. Only flags if the difference is significant (≥2 points)

This threshold prevents false positives from normal variation while catching real patterns.

**Mistake Patterns**: I track mistake frequency because recurring mistakes are opportunities for improvement. The system:
1. Groups mistakes by category
2. Counts occurrences in a 30-day window
3. Flags categories that appear 3+ times

The 30-day window balances recency with statistical significance. Too short, and you get noise. Too long, and patterns become stale.

**Mood/Sleep Patterns**: These use average calculations over 2-week periods. Two weeks is long enough to smooth out daily variation but short enough to catch recent trends. The thresholds (≤4 for mood, ≤5 for sleep) are based on common mental health assessment scales.

## UI Design Decisions

**Sliders for Numeric Input**: I use sliders instead of text boxes for 1-10 scales because:
- Visual feedback is immediate
- Harder to enter invalid values
- More intuitive for rating scales
- Shows the full range of possible values

**GroupBoxes for Organization**: The UI has many fields. GroupBoxes create visual hierarchy and make it clear which fields are related. This is especially important for users with ADHD who might find overwhelming UIs difficult to navigate.

**ScrollViewers**: All views are wrapped in ScrollViewers because:
- Content can vary significantly (some users might have many habits, medications, etc.)
- Prevents UI from breaking on smaller screens
- Allows for future expansion without layout issues

**Radio Buttons for Yes/No/N/A**: Some questions have three valid states: Yes, No, and Not Applicable. Using radio buttons makes all options visible and explicit. The converters handle the nullable boolean binding.

## Error Handling Philosophy

I believe in defensive programming. The code assumes things can go wrong and handles it gracefully:

- File operations: Try-catch around all file I/O
- Notifications: Fallback to MessageBox if toast notifications fail
- Null checks: Extensive use of null-conditional operators
- Default values: Models initialize with sensible defaults
- Comprehensive logging: All exceptions are logged with full context

This prevents the application from crashing and provides a better user experience. A journaling app should be reliable - users need to trust that their data is safe.

## Logging: Making Debugging Possible

When I first built the application, it would crash on startup with no indication of why. This is a common problem in desktop applications - errors can occur silently, leaving developers (and users) frustrated.

I implemented a comprehensive logging system using Serilog because:

1. **Visibility**: I needed to see what was happening during application startup, service initialization, and data operations
2. **Debugging**: When something goes wrong, logs provide the context needed to understand why
3. **Production Support**: Even after deployment, logs help diagnose issues without requiring a debugger

**Why Serilog?**
- Industry standard in .NET applications
- Excellent file and console sinks
- Structured logging capabilities (for future expansion)
- Easy to configure and extend
- Good performance (doesn't slow down the application)

**Implementation Approach**:
I created an `ILoggerService` interface to abstract the logging implementation. This follows the Dependency Inversion Principle - components depend on the abstraction, not the concrete implementation. If I ever need to switch logging frameworks, I only change the implementation, not the code that uses it.

The logger is registered first in the dependency injection container, ensuring it's available to all other services during initialization. This is critical - if a service fails during construction, we want to log that failure.

**What Gets Logged**:
- Application lifecycle events (startup, shutdown)
- Service initialization (success and failures)
- Data operations (loading, saving, errors)
- Navigation events
- All exceptions with full stack traces

**Log File Management**:
- Logs are written to `%LocalAppData%\DailyCheckInJournal\Logs\`
- Files are rotated daily (one file per day)
- 30 days of history are retained (older logs are automatically deleted)
- This prevents log files from consuming too much disk space

**Global Exception Handlers**:
I added handlers for both `AppDomain.UnhandledException` and `Dispatcher.UnhandledException`. This ensures that even if an exception escapes all try-catch blocks, it's still logged. The dispatcher handler also prevents the application from crashing immediately, giving users a chance to save their work.

This logging system was added after initial development, which demonstrates my ability to add infrastructure features to an existing codebase without disrupting the architecture. The dependency injection system made this straightforward - I just added the logger service and injected it where needed.

## Why These Technologies?

**C# / .NET 8**: 
- Strong typing catches errors at compile time
- Excellent tooling (Visual Studio, debugging, profiling)
- Rich ecosystem of libraries
- Good performance
- Cross-platform capabilities (though this app targets Windows)

**WPF**:
- Mature, stable framework
- Powerful data binding
- Rich UI capabilities
- Good third-party library support
- XAML separates UI from code

**CommunityToolkit.Mvvm**:
- Reduces boilerplate significantly
- Well-maintained by Microsoft
- Follows MVVM best practices
- Active community

**LiveCharts**:
- Modern, clean API
- Good performance
- Active development
- Free and open-source
- Good WPF integration

**Newtonsoft.Json**:
- Industry standard
- Handles complex scenarios well
- Good error messages
- Widely used and trusted

**Serilog**:
- Comprehensive logging framework
- Excellent file and console sinks
- Structured logging capabilities
- Easy to configure
- Good performance
- Widely adopted in .NET ecosystem

## Code Organization

The folder structure reflects the architecture:

```
Models/          - Data structures
Services/        - Business logic and data access
ViewModels/      - Presentation logic
Views/           - UI (XAML)
Converters/      - Value converters
Controls/        - Custom controls
Themes/          - Visual styling
```

This organization makes it easy to:
- Find code (you know where to look)
- Understand dependencies (they flow in one direction)
- Add new features (follow the existing patterns)

## Testing Strategy (Future Work)

While I haven't implemented unit tests yet, the architecture supports them:

- Services can be tested with mock data services
- ViewModels can be tested with mock services
- Pattern detection algorithms can be tested with known data sets
- Converters can be tested in isolation

The dependency injection setup makes it straightforward to inject test doubles. This is a priority for future development.

## Performance Considerations

I've made several performance-conscious decisions:

- **Async/Await**: All file I/O is asynchronous to keep the UI responsive
- **Lazy Loading**: Data is loaded on-demand (when a view is opened)
- **Efficient Queries**: LINQ queries are optimized (using Where before Select, etc.)
- **Singleton Services**: Services that don't need multiple instances are singletons

For a single-user desktop application, performance isn't usually a bottleneck, but I still wanted to follow best practices.

## Accessibility Considerations

I designed the UI with accessibility in mind:

- **High Contrast Theme**: For users with visual impairments
- **Clear Labels**: All inputs have descriptive labels
- **Keyboard Navigation**: Standard WPF controls support keyboard navigation
- **Scalable UI**: Uses relative sizing where possible

The customizable themes also help users find a visual style that works for them, which is important for sensory sensitivities common in autism.

## Security & Privacy

- **Local Storage**: Data stays on the user's machine by default
- **Optional Encryption**: Framework for password protection is in place
- **Google Drive**: Opt-in only, requires explicit user authentication
- **No Telemetry**: The app doesn't collect or send any data

For a personal journaling app, privacy is paramount. Users need to trust that their sensitive mental health data is secure.

## What I Learned

Building this project reinforced several lessons:

1. **Start Simple**: I could have over-engineered the pattern detection, but starting with simple algorithms and iterating is more effective.

2. **User Experience Matters**: A feature that's hard to use is a feature that won't be used. The UI needs to be intuitive, especially for users who might be experiencing overwhelm or executive function challenges.

3. **Code is Read More Than Written**: I write code thinking about the next person who will read it (often future me). Clear naming, good organization, and thoughtful comments make maintenance easier.

4. **Architecture Enables Features**: Good architecture doesn't just make code cleaner - it makes new features easier to add. When I wanted to add pattern detection, the service layer made it straightforward.

## Conclusion

This project represents my approach to software development: analytical, methodical, and focused on building solid foundations. Every decision was made with specific reasoning, and the architecture reflects that thought process.

The bottom-up approach I used - starting with data models and building up through services, ViewModels, and views - ensures that each layer is well-understood before moving to the next. This creates a codebase that's not just functional, but maintainable and extensible.

I'm proud of how the pattern detection system turned out. It demonstrates my ability to take raw data and extract meaningful insights - a skill that's valuable in many domains beyond mental health tracking.

The codebase is ready for future enhancements, and the architecture supports growth without requiring major refactoring. That's the sign of a well-designed system.

