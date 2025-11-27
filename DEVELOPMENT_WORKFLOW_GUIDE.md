# Development Workflow Practice Guide

**Purpose**: This guide helps you practice real development workflows (like you'd use at work) with this project.

**How to Use**: Work through each section ONE AT A TIME. Don't move to the next section until you've completed the current one.

---

## 📋 Table of Contents

1. [Setting Up Git & GitHub](#1-setting-up-git--github)
2. [Creating Your First Story](#2-creating-your-first-story)
3. [Branching Strategy](#3-branching-strategy)
4. [Making Changes & Commits](#4-making-changes--commits)
5. [Creating Pull Requests](#5-creating-pull-requests)
6. [Code Review Practice](#6-code-review-practice)
7. [Time Tracking](#7-time-tracking)
8. [Updating Stories](#8-updating-stories)

---

## 1. Setting Up Git & GitHub

### Step 1.1: Check if Git is initialized
**What to do**: Open PowerShell in your project folder and run:
```powershell
git status
```

**What you'll see**:
- ✅ If it shows files → Git is already set up (skip to Step 1.3)
- ❌ If it says "not a git repository" → Go to Step 1.2

### Step 1.2: Initialize Git (if needed)
**What to do**: Run these commands one at a time:
```powershell
git init
git add .
git commit -m "Initial commit: Wellness Journal project"
```

**What this does**: Creates a git repository and saves your current code.

### Step 1.3: Create a GitHub repository
**What to do**:
1. Go to https://github.com/new
2. Repository name: `CSharpWellnessJournal` (or whatever you want)
3. Description: "Daily wellness journal app for tracking mental health"
4. Choose: **Private** (so it's just for you)
5. **DO NOT** check "Initialize with README" (you already have one)
6. Click "Create repository"

### Step 1.4: Connect your local project to GitHub
**What to do**: GitHub will show you commands. Copy and run these (replace YOUR_USERNAME):
```powershell
git remote add origin https://github.com/YOUR_USERNAME/CSharpWellnessJournal.git
git branch -M main
git push -u origin main
```

**What this does**: Connects your local code to GitHub and uploads it.

**✅ Checkpoint**: You should see your code on GitHub now!

---

## 2. Creating Your First Story

### Step 2.1: Choose a small feature to work on
**Examples of good "first stories"**:
- Add a "Clear All Data" button in Settings
- Add keyboard shortcuts (Ctrl+S to save)
- Improve error messages
- Add a "About" dialog box

**Pick ONE** that sounds interesting to you.

### Step 2.2: Create a GitHub Issue for your story
**What to do**:
1. Go to your GitHub repository
2. Click the "Issues" tab
3. Click "New Issue"
4. Fill it out like this:

**Title**: `[Feature] Add Clear All Data button in Settings`

**Description** (copy this template):
```
## Description
Add a button in the Settings view that allows users to clear all their journal data.

## Acceptance Criteria
- [ ] Button is visible in Settings view
- [ ] Button shows a confirmation dialog before clearing
- [ ] All data files are deleted when confirmed
- [ ] User sees a success message after clearing
- [ ] App handles the case where data is cleared gracefully

## Estimated Time
2 hours

## Notes
This will help users who want to start fresh or test the app.
```

5. Click "Submit new issue"
6. **Write down the issue number** (it will be #1, #2, etc.)

**✅ Checkpoint**: You now have your first story/ticket!

---

## 3. Branching Strategy

### Step 3.1: Create a branch for your story
**What to do**: In PowerShell, run (replace #1 with your issue number):
```powershell
git checkout -b feature/clear-all-data-button
```

**Branch naming rules**:
- `feature/` = new feature
- `bugfix/` = fixing a bug
- `hotfix/` = urgent fix
- Use lowercase and hyphens: `feature/my-new-feature`

**What this does**: Creates a new branch and switches to it.

### Step 3.2: Verify you're on the new branch
**What to do**: Run:
```powershell
git branch
```

**What you'll see**: The current branch has a `*` next to it. Make sure it's your new branch!

**✅ Checkpoint**: You're now on a feature branch!

---

## 4. Making Changes & Commits

### Step 4.1: Make your code changes
**What to do**: 
- Open your code editor
- Make the changes needed for your story
- Test that it works
- **Don't worry about perfection** - you can always improve it later!

### Step 4.2: Stage your changes
**What to do**: Run:
```powershell
git add .
```

**What this does**: Tells Git "I want to save these changes."

### Step 4.3: Commit your changes
**What to do**: Run (replace with your actual changes):
```powershell
git commit -m "feat: add clear all data button in settings

- Added ClearAllDataCommand to SettingsViewModel
- Added button to SettingsView.xaml
- Implemented confirmation dialog
- Added success message after clearing

Closes #1"
```

**Commit message format**:
- First line: Short summary (50 chars or less)
- Blank line
- Bullet points explaining what changed
- "Closes #1" links to your GitHub issue

**Commit types**:
- `feat:` = new feature
- `fix:` = bug fix
- `docs:` = documentation
- `refactor:` = code cleanup
- `test:` = adding tests

**✅ Checkpoint**: Your changes are saved locally!

---

## 5. Creating Pull Requests

### Step 5.1: Push your branch to GitHub
**What to do**: Run:
```powershell
git push -u origin feature/clear-all-data-button
```

**What this does**: Uploads your branch to GitHub.

### Step 5.2: Create the Pull Request
**What to do**:
1. Go to your GitHub repository
2. You'll see a yellow banner saying "Compare & pull request" - click it
3. Fill out the PR template:

**Title**: `[Feature] Add Clear All Data button in Settings`

**Description** (copy this template):
```markdown
## Description
This PR adds a "Clear All Data" button to the Settings view, allowing users to delete all their journal data.

## Related Issue
Closes #1

## Changes Made
- Added `ClearAllDataCommand` to `SettingsViewModel`
- Added button to `SettingsView.xaml`
- Implemented confirmation dialog using `MessageBox`
- Added success message after data is cleared
- Updated `DataService` to handle clearing all data

## How to Test
1. Open the Settings view
2. Click "Clear All Data" button
3. Confirm the dialog
4. Verify all data files are deleted
5. Verify success message appears

## Screenshots
(Add a screenshot if it's a UI change)

## Time Spent
2.5 hours (estimated 2 hours)
```

4. Click "Create pull request"

**✅ Checkpoint**: Your PR is created!

---

## 6. Code Review Practice

### Step 6.1: Review your own PR
**What to do**: 
1. Look at your PR on GitHub
2. Click "Files changed" tab
3. Read through your code as if you're reviewing someone else's work
4. Ask yourself:
   - Is the code readable?
   - Are there any obvious bugs?
   - Could anything be simplified?
   - Are variable names clear?

### Step 6.2: Add review comments (to yourself)
**What to do**:
1. Click on a line of code
2. Click the "+" button that appears
3. Write a comment like: "Maybe we should add error handling here?"
4. Click "Add single comment"

### Step 6.3: Address the comments
**What to do**:
1. Make the changes in your code
2. Commit and push:
```powershell
git add .
git commit -m "refactor: add error handling to clear data method

Addresses review comment about error handling"
git push
```

**What this does**: The PR automatically updates with your new changes!

### Step 6.4: Respond to comments
**What to do**: 
1. Go back to your PR
2. Find your comment
3. Reply: "Good catch! I've added error handling in the latest commit."
4. Click "Resolve conversation" when done

**✅ Checkpoint**: You've practiced the full review cycle!

---

## 7. Time Tracking

### Step 7.1: Create a time tracking template
**What to do**: Create a file called `TIME_TRACKING.md` in your project:

```markdown
# Time Tracking Log

## Story: Add Clear All Data Button (#1)

| Date | Task | Time Spent | Notes |
|------|------|------------|-------|
| 2024-01-15 | Planning & reading code | 0.5h | Understanding SettingsViewModel |
| 2024-01-15 | Implementing button | 1.0h | Added button and command |
| 2024-01-15 | Testing & debugging | 0.5h | Fixed confirmation dialog |
| 2024-01-15 | Code review & improvements | 0.5h | Added error handling |
| **Total** | | **2.5h** | Estimated: 2h |

## Story: [Next Story Name] (#2)

(Add new entries as you work on new stories)
```

### Step 7.2: Track time as you work
**What to do**: 
- Before you start working: Write down the time
- When you finish: Write down how long it took
- Be honest - it's just for practice!

**Tips**:
- Use a timer if it helps
- Round to 15-minute increments (0.25h, 0.5h, 0.75h, 1h)
- Note what took longer than expected

**✅ Checkpoint**: You're tracking your time!

---

## 8. Updating Stories

### Step 8.1: Update story status
**What to do**: 
1. Go to your GitHub Issue
2. Add comments as you work:
   - "Starting work on this..."
   - "Ran into an issue with X, investigating..."
   - "PR created: #2"
   - "PR merged, closing this issue"

### Step 8.2: Link PR to Issue
**What to do**: 
- In your PR description, include: `Closes #1`
- When the PR is merged, GitHub automatically closes the issue!

### Step 8.3: Update time estimates
**What to do**: 
- In your issue, add a comment: "Actual time: 2.5h (estimated 2h)"
- This helps you get better at estimating!

### Step 8.4: Close the issue
**What to do**: 
- Once your PR is merged, the issue will auto-close
- Or manually click "Close issue" if needed

**✅ Checkpoint**: Story is complete!

---

## 🎯 Next Steps

Once you've completed one full cycle (Story → Branch → PR → Review → Merge), you're ready to do it again with a new story!

**Remember**:
- ✅ One thing at a time
- ✅ It's okay to make mistakes
- ✅ Practice makes perfect
- ✅ You're building real skills!

---

## 📚 Quick Reference

### Common Git Commands
```powershell
git status                    # See what files changed
git add .                     # Stage all changes
git commit -m "message"       # Save changes
git push                      # Upload to GitHub
git checkout -b feature/name  # Create new branch
git checkout main             # Switch to main branch
```

### Branch Naming
- `feature/description` - New features
- `bugfix/description` - Bug fixes
- `hotfix/description` - Urgent fixes

### Commit Types
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation
- `refactor:` - Code cleanup
- `test:` - Tests

---

**Questions?** Take it one step at a time. You've got this! 💪

