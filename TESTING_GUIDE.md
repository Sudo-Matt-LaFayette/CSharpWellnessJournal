# Testing Guide

## Quick Start

To run the application:
```bash
cd "C:\Users\lafay\Desktop\Projects\Vibe Coding Projects\CSharpWellnessJournal"
dotnet run
```

Or open the project in Visual Studio and press F5.

## What to Test

### 1. Morning Check-In
- Navigate to "Morning Check-In" from the main window
- Fill out all fields:
  - Energy level (slider 1-10)
  - Must-do task (text input)
  - Capacity feeling (text input)
  - Sleep quality from last night (slider 1-10)
  - Medications (add/remove entries)
  - Sensory state (overload level, triggers, environmental factors)
  - Executive function (task initiation, transitions, time blindness, hyperfocus)
  - Emotional state (mood, meltdowns, shutdowns, overwhelm, what helped)
  - Habits (check off completed habits)
  - Gratitude/positives (text input)
  - Triggers (add entries with notes)
  - Coping strategies (add entries with notes)
- Click "Save Morning Check-In"
- Verify data is saved (close and reopen the app, check that data persists)

### 2. Evening Check-In
- Navigate to "Evening Check-In"
- Verify it loads today's morning check-in data (if completed)
- Fill out evening-specific fields:
  - Did must-do happen? (Yes/No/N/A)
  - Energy level now (slider 1-10)
  - Did you overcommit? (Yes/No/N/A)
  - Common mistakes (add entries with category, tags, notes)
  - Update emotional state
  - Update habit completion
  - Add evening gratitude
  - Review/update triggers and coping strategies
- Click "Save Evening Check-In"
- Verify data persists

### 3. Weekly Review
- Navigate to "Weekly Review"
- Select a date range (use the date pickers)
- Review statistics:
  - Average energy levels
  - Average mood
  - Must-do completion rate
  - Overcommitment frequency
  - Sleep quality averages
- Check pattern detection:
  - Patterns should appear after 7+ days of data
  - Review detected patterns (energy, mood, sleep, mistakes, overcommitment)
- Generate weekly report (click "Generate Report")
- Verify report displays correctly

### 4. Visualizations
- Navigate to "Visualizations"
- Select different chart types:
  - Energy Trends
  - Mood Patterns
  - Sleep Quality
  - Mistake Frequency
- Adjust date range filters
- Verify charts update correctly
- Check that charts display data when available

### 5. Settings
- Navigate to "Settings"
- **Reminders**:
  - Set morning reminder time (use the time picker)
  - Set evening reminder time
  - Enable/disable system notifications
  - Enable/disable in-app notifications
  - Click "Save Settings"
- **Theme**:
  - Select different themes from the dropdown
  - Verify theme changes immediately
  - Try: Default, Dark, Light, High Contrast, Autumn, Ocean
- **Habits**:
  - Add a custom habit
  - Remove a habit (custom ones only)
  - Verify habits appear in check-in views
- **Mistake Categories**:
  - Add a custom mistake category
  - Remove a category
  - Verify categories appear when adding mistakes
- **Google Drive Sync** (if configured):
  - Click "Authenticate with Google Drive"
  - Complete OAuth flow
  - Test sync functionality

### 6. Navigation
- Test all navigation buttons:
  - Morning Check-In
  - Evening Check-In
  - Weekly Review
  - Visualizations
  - Settings
- Verify views switch correctly
- Check that navigation buttons are clearly visible

### 7. Data Persistence
- Complete a morning check-in
- Close the application completely
- Reopen the application
- Navigate to Morning Check-In
- Verify your data is still there
- Complete an evening check-in
- Close and reopen
- Verify both morning and evening data persist

### 8. Notifications (if enabled)
- Set reminder times in Settings
- Wait for reminder time (or set it to a minute from now for testing)
- Verify you receive a notification (system toast or in-app)
- Check that notifications appear at the correct times

### 9. Pattern Detection
- Create at least 7 days of check-in data (you can backdate by modifying the date in the check-in views)
- Navigate to Weekly Review
- Verify patterns are detected:
  - Energy patterns (if there's a day-of-week pattern)
  - Mistake patterns (if mistakes occur 3+ times in 30 days)
  - Mood patterns (if average mood drops below 4/10)
  - Sleep patterns (if average sleep quality drops below 5/10)
  - Overcommitment patterns (if certain days have frequent overcommitment)

### 10. Edge Cases
- Try saving a check-in without filling required fields (should work - most fields are optional)
- Try navigating between views rapidly
- Test with no data (fresh install)
- Test with large amounts of data (many check-ins)
- Try adding many habits and mistake categories
- Test date range selection in Weekly Review and Visualizations

## Common Issues to Watch For

1. **Data Not Saving**: Check that the app has write permissions to `%LocalAppData%\DailyCheckInJournal\`
2. **Notifications Not Appearing**: Verify Windows notification settings allow the app to send notifications
3. **Charts Not Displaying**: Ensure you have data for the selected date range
4. **Theme Not Changing**: Try restarting the app after changing themes
5. **Patterns Not Detecting**: Ensure you have at least 7 days of data
6. **App Crashes**: Check the log files in `%LocalAppData%\DailyCheckInJournal\Logs\` for error details and stack traces

## Data Location

Your data is stored in:
```
%LocalAppData%\DailyCheckInJournal\
```

On Windows, this is typically:
```
C:\Users\[YourUsername]\AppData\Local\DailyCheckInJournal\
```

Files:
- `checkins.json` - All check-in data
- `goals.json` - Goal tracking data
- `habits.json` - Habit definitions
- `patterns.json` - Detected patterns
- `settings.json` - Application settings

You can manually inspect these files (they're JSON) to verify data is being saved correctly.

## Debugging Tips

If something isn't working:
1. **Check the log files**: The most important debugging tool. Logs are in `%LocalAppData%\DailyCheckInJournal\Logs\app-YYYYMMDD.log`. Look for `[ERR]` entries and stack traces.
2. Check the console output when running `dotnet run` - errors will appear there in real-time
3. Check the data files in `%LocalAppData%\DailyCheckInJournal\` to see if data is being saved
4. Try deleting the data files and starting fresh
5. Check Windows Event Viewer for application errors
6. Verify all NuGet packages are installed: `dotnet restore`
7. See `LOGGING.md` for detailed information about the logging system

## Performance Testing

- Test with 30+ days of check-in data
- Verify charts render smoothly with large datasets
- Check that pattern detection completes in reasonable time
- Ensure UI remains responsive during data operations

