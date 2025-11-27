# 🚀 Start Here - Your First Workflow Practice

**Welcome!** This guide will walk you through your FIRST complete development workflow cycle, step by step.

**Time needed**: About 30-60 minutes (depending on how fast you work)

**What you'll do**: Fix a small bug or add a tiny feature, following the same process you'd use at work.

---

## ✅ Step 1: Check if Git is Set Up (2 minutes)

1. Open PowerShell
2. Navigate to your project folder:
   ```powershell
   cd "C:\Users\lafay\Desktop\Projects\Vibe Coding Projects\CSharpWellnessJournal"
   ```
3. Run this command:
   ```powershell
   git status
   ```

**What happens next?**
- ✅ **If you see a list of files**: Git is set up! Go to Step 2.
- ❌ **If you see "not a git repository"**: Run these commands:
   ```powershell
   git init
   git add .
   git commit -m "Initial commit"
   ```
   Then go to Step 2.

---

## ✅ Step 2: Create Your First Story (5 minutes)

**We'll start with something SUPER simple**: Add a "Version" label to the About/Settings area.

1. Go to https://github.com (log in if needed)
2. Click the "+" icon in the top right
3. Click "New repository"
4. Fill it out:
   - **Name**: `CSharpWellnessJournal`
   - **Description**: "Daily wellness journal app"
   - **Visibility**: Choose **Private**
   - **DO NOT** check "Add a README file"
5. Click "Create repository"

**Now create your first issue:**
1. In your new repository, click the "Issues" tab
2. Click "New issue"
3. Use the "Feature Request" template (if it shows up)
4. Fill it out like this:

   **Title**: `[Feature] Add version number display in Settings`
   
   **Description**:
   ```
   Add a label in the Settings view that shows the app version number.
   
   ## Acceptance Criteria
   - [ ] Version label is visible in Settings view
   - [ ] Version number is correct (e.g., "Version 1.0.0")
   - [ ] Label is styled consistently with the rest of the UI
   
   ## Estimated Time
   1 hour
   ```

5. Click "Submit new issue"
6. **Write down the issue number** (it will be #1)

**✅ Done with Step 2!** You now have your first story.

---

## ✅ Step 3: Create a Branch (2 minutes)

1. Go back to PowerShell (in your project folder)
2. Run this command:
   ```powershell
   git checkout -b feature/add-version-display
   ```
3. Verify it worked:
   ```powershell
   git branch
   ```
   You should see a `*` next to `feature/add-version-display`

**✅ Done with Step 3!** You're now on a feature branch.

---

## ✅ Step 4: Make the Change (10-15 minutes)

**What to do**: Add a version label to the Settings view.

1. Open your project in your code editor
2. Find the Settings view file (probably `Views/SettingsView.xaml`)
3. Add a TextBlock or Label that shows the version
4. You might need to:
   - Add a property to SettingsViewModel for the version
   - Or hardcode it for now (that's fine for practice!)

**Example** (in SettingsView.xaml, add this somewhere visible):
```xml
<TextBlock Text="Version 1.0.0" 
           FontSize="12" 
           Foreground="Gray"
           HorizontalAlignment="Center"
           Margin="0,10,0,0"/>
```

5. **Test it**: Run the app and make sure it shows up!

**✅ Done with Step 4!** Your change is ready.

---

## ✅ Step 5: Commit Your Changes (2 minutes)

1. In PowerShell, run:
   ```powershell
   git add .
   git status
   ```
   (This shows you what will be committed)

2. Commit your changes:
   ```powershell
   git commit -m "feat: add version number display in settings

   - Added version label to SettingsView
   - Displays version 1.0.0
   
   Closes #1"
   ```

**✅ Done with Step 5!** Changes are saved.

---

## ✅ Step 6: Push to GitHub (2 minutes)

**First time only**: Connect your local project to GitHub

1. In PowerShell, run (replace YOUR_USERNAME with your GitHub username):
   ```powershell
   git remote add origin https://github.com/YOUR_USERNAME/CSharpWellnessJournal.git
   git branch -M main
   git push -u origin main
   ```

2. Then push your feature branch:
   ```powershell
   git push -u origin feature/add-version-display
   ```

**If you already connected**: Just run:
```powershell
git push
```

**✅ Done with Step 6!** Your code is on GitHub.

---

## ✅ Step 7: Create Pull Request (5 minutes)

1. Go to your GitHub repository
2. You should see a yellow banner saying "Compare & pull request" - click it!
3. Fill out the PR:

   **Title**: `[Feature] Add version number display in Settings`
   
   **Description** (copy this):
   ```markdown
   ## Description
   Adds a version number label to the Settings view.
   
   ## Related Issue
   Closes #1
   
   ## Changes Made
   - Added version label to SettingsView.xaml
   - Displays "Version 1.0.0"
   
   ## How to Test
   1. Open the app
   2. Navigate to Settings
   3. Verify version number is displayed
   
   ## Time Spent
   0.5 hours (estimated: 1 hour)
   ```

4. Click "Create pull request"

**✅ Done with Step 7!** Your PR is created!

---

## ✅ Step 8: Review Your Own Code (5 minutes)

1. In your PR, click the "Files changed" tab
2. Look at your code
3. Click on a line of code
4. Click the "+" button that appears
5. Add a comment like: "Maybe we should make this configurable?"
6. Click "Add single comment"

**Now address the comment:**
1. Make a small improvement (or just acknowledge it)
2. Commit and push:
   ```powershell
   git add .
   git commit -m "refactor: improve version display formatting"
   git push
   ```
3. Go back to your PR and reply to your comment: "Good point! I've improved the formatting."

**✅ Done with Step 8!** You've practiced code review!

---

## ✅ Step 9: Merge Your PR (1 minute)

1. In your PR, scroll to the bottom
2. Click "Merge pull request"
3. Click "Confirm merge"
4. Click "Delete branch" (clean up)

**✅ Done with Step 9!** Your code is merged!

---

## ✅ Step 10: Update Time Tracking (2 minutes)

1. Open `TIME_TRACKING.md` in your project
2. Add an entry for your story:

```markdown
## Story: Add version number display (#1)

| Date | Task | Time Spent | Notes |
|------|------|------------|-------|
| 2024-01-15 | Implementation | 0.5h | Added version label |
| **Total** | | **0.5h** | **Estimated: 1h** |

**Lessons Learned**: 
- This was simpler than I thought!
- Next time I'll estimate 0.5h for similar tasks
```

**✅ Done with Step 10!** You've completed your first cycle!

---

## 🎉 Congratulations!

You just completed a full development workflow cycle:
- ✅ Created a story
- ✅ Created a branch
- ✅ Made changes
- ✅ Created a PR
- ✅ Did code review
- ✅ Merged the code
- ✅ Tracked your time

**You're ready to do it again with a new story!**

---

## 📚 What's Next?

1. Pick another small feature or bug fix
2. Follow the same steps
3. Use `WORKFLOW_CHECKLIST.md` to make sure you don't miss anything
4. Reference `DEVELOPMENT_WORKFLOW_GUIDE.md` for detailed explanations

**Remember**: 
- Take it one step at a time
- It's okay to make mistakes
- Practice makes perfect!

---

## ❓ Need Help?

If you get stuck:
1. Check `DEVELOPMENT_WORKFLOW_GUIDE.md` for detailed explanations
2. Google the error message
3. Take a break and come back to it

**You've got this!** 💪

