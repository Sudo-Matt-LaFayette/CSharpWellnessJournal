# CI/CD with GitHub Actions - Interactive Learning Guide

## What You'll Learn

By the end of this guide, you will have:
- ✅ Set up a GitHub Actions workflow that builds your .NET project
- ✅ Configured automated testing (when you add tests)
- ✅ Created a release workflow
- ✅ Understood how CI/CD works in practice

## Learning Style: Hands-On Practice

This guide follows a **"Learn by Doing"** approach. You'll:
1. **Read** a concept
2. **Do** the action yourself
3. **Verify** it worked
4. **Understand** what happened

Think of it like learning to change a car's oil - you can read about it, but you truly learn when you get your hands dirty!

---

## Part 1: Understanding CI/CD (5 minutes)

### What is CI/CD?

**CI (Continuous Integration)**: Automatically building and testing your code every time you push changes.

**CD (Continuous Deployment/Delivery)**: Automatically deploying your application after successful builds.

### Why Use It?

- ✅ **Catch bugs early** - Find problems before they reach production
- ✅ **Save time** - No manual building/testing
- ✅ **Consistency** - Same build process every time
- ✅ **Confidence** - Know your code works before merging

### For Your Project

Right now, you manually:
1. Write code
2. Build locally (`dotnet build`)
3. Test manually
4. Create releases manually

With CI/CD, GitHub will:
1. Detect your code changes
2. Build automatically
3. Run tests automatically
4. Create releases automatically

---

## Part 2: Your First Workflow (Let's Do It!)

### Step 1: Create the Workflow Directory

**🎯 ACTION REQUIRED:**

1. In your project root, create this folder structure:
   ```
   .github/
     workflows/
   ```

2. **Do it now:**
   - Create `.github` folder (note the dot at the start - it's a hidden folder)
   - Inside `.github`, create `workflows` folder

**💡 TIP:** In Windows Explorer, you might need to enable "Show hidden files" to see the `.github` folder.

**✅ CHECKPOINT:** You should have `.github/workflows/` folder structure.

---

### Step 2: Create Your First Workflow File

**🎯 ACTION REQUIRED:**

1. Create a file named `build.yml` inside `.github/workflows/`

2. **Do it now:**
   - Right-click in the `workflows` folder
   - Create new file: `build.yml`

**✅ CHECKPOINT:** You have `.github/workflows/build.yml` file (it's empty right now, that's okay!)

---

### Step 3: Write Your First Workflow

**🎯 ACTION REQUIRED:**

Copy this content into `build.yml`:

```yaml
name: Build and Test

on:
  push:
    branches: [ main, master ]
  pull_request:
    branches: [ main, master ]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
      
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore
      
    - name: Test (if tests exist)
      run: dotnet test --no-build --verbosity normal
      continue-on-error: true
```

**📝 EXPLANATION:**

Let's break down what each part does:

- `name: Build and Test` - The name of your workflow (shows in GitHub UI)
- `on: push:` - Triggers when you push code
- `branches: [ main, master ]` - Only runs on these branches
- `jobs: build:` - Defines a job called "build"
- `runs-on: windows-latest` - Uses Windows (needed for WPF)
- `steps:` - List of actions to perform
- `uses: actions/checkout@v4` - Downloads your code
- `uses: actions/setup-dotnet@v4` - Installs .NET 8.0
- `run: dotnet restore` - Downloads NuGet packages
- `run: dotnet build` - Builds your project
- `run: dotnet test` - Runs tests (if any exist)

**✅ CHECKPOINT:** Your `build.yml` file has the workflow content.

---

### Step 4: Commit and Push

**🎯 ACTION REQUIRED:**

1. **Stage your changes:**
   ```bash
   git add .github/workflows/build.yml
   ```

2. **Commit:**
   ```bash
   git commit -m "Add GitHub Actions build workflow"
   ```

3. **Push to your branch:**
   ```bash
   git push origin AddOptionToViewDays
   ```

**✅ CHECKPOINT:** Your code is pushed to GitHub.

---

### Step 5: Watch the Magic Happen!

**🎯 ACTION REQUIRED:**

1. Go to your GitHub repository in a web browser
2. Click on the **"Actions"** tab (top navigation)
3. You should see your workflow running!

**What to look for:**
- A yellow circle = running
- A green checkmark = success ✅
- A red X = failure ❌

4. Click on the workflow run to see details
5. Click on the "build" job to see each step

**🎉 CONGRATULATIONS!** You just ran your first CI/CD pipeline!

**✅ CHECKPOINT:** You can see the workflow running in GitHub Actions tab.

---

## Part 3: Understanding What Happened

### What GitHub Did:

1. **Detected your push** - GitHub saw you pushed code
2. **Started a virtual machine** - Created a Windows server
3. **Downloaded your code** - Checked out your repository
4. **Installed .NET** - Set up the .NET 8.0 SDK
5. **Restored packages** - Downloaded NuGet packages
6. **Built your project** - Compiled your code
7. **Ran tests** - Tried to run tests (skipped if none exist)

### Common Issues:

**❌ Build Failed?**
- Check the error message in the Actions tab
- Common issues:
  - Missing dependencies
  - Build errors in your code
  - Wrong .NET version

**❌ Can't see Actions tab?**
- Make sure you pushed the `.github/workflows/` folder
- Check that the file is named correctly (`build.yml`)

---

## Part 4: Making It Better (Advanced Steps)

### Step 6: Add Build Artifacts

**🎯 ACTION REQUIRED:**

Update your `build.yml` to save the built application:

```yaml
name: Build and Test

on:
  push:
    branches: [ main, master ]
  pull_request:
    branches: [ main, master ]

jobs:
  build:
    runs-on: windows-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
      
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore --configuration Release
      
    - name: Publish
      run: dotnet publish -c Release -o ./publish --no-build
      
    - name: Upload build artifacts
      uses: actions/upload-artifact@v4
      with:
        name: DailyCheckInJournal
        path: ./publish
        retention-days: 7
```

**📝 EXPLANATION:**

- `--configuration Release` - Builds in Release mode (optimized)
- `dotnet publish` - Creates a distributable version
- `upload-artifact` - Saves the build so you can download it
- `retention-days: 7` - Keeps artifacts for 7 days

**✅ CHECKPOINT:** After pushing, you can download the built app from the Actions tab!

---

### Step 7: Add Multiple Build Configurations

**🎯 ACTION REQUIRED:**

Create a matrix build to test multiple .NET versions (optional):

```yaml
name: Build and Test

on:
  push:
    branches: [ main, master ]
  pull_request:
    branches: [ main, master ]

jobs:
  build:
    runs-on: windows-latest
    strategy:
      matrix:
        dotnet-version: ['8.0.x']
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
      
    - name: Setup .NET ${{ matrix.dotnet-version }}
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: ${{ matrix.dotnet-version }}
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --no-restore --configuration Release
```

**📝 EXPLANATION:**

- `strategy: matrix:` - Runs the job multiple times with different values
- `${{ matrix.dotnet-version }}` - Uses the variable from the matrix
- This lets you test against multiple .NET versions easily

---

## Part 5: Creating Releases (CD - Continuous Deployment)

### Step 8: Create a Release Workflow

**🎯 ACTION REQUIRED:**

Create a new file: `.github/workflows/release.yml`

```yaml
name: Create Release

on:
  push:
    tags:
      - 'v*.*.*'  # Triggers on tags like v1.0.0, v1.2.3, etc.

jobs:
  release:
    runs-on: windows-latest
    
    steps:
    - name: Checkout code
      uses: actions/checkout@v4
      
    - name: Setup .NET
      uses: actions/setup-dotnet@v4
      with:
        dotnet-version: '8.0.x'
        
    - name: Restore dependencies
      run: dotnet restore
      
    - name: Build
      run: dotnet build --configuration Release --no-restore
      
    - name: Publish
      run: dotnet publish -c Release -o ./publish --no-build
      
    - name: Create Release
      uses: softprops/action-gh-release@v1
      with:
        files: ./publish/**
        draft: false
        prerelease: false
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

**📝 EXPLANATION:**

- `tags: - 'v*.*.*'` - Only runs when you create a version tag
- `action-gh-release` - Creates a GitHub release
- `files: ./publish/**` - Uploads all published files
- `GITHUB_TOKEN` - Automatically provided by GitHub

**✅ CHECKPOINT:** You have a release workflow ready!

---

### Step 9: Test the Release Workflow

**🎯 ACTION REQUIRED:**

1. **Create a version tag:**
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```

2. **Watch it run:**
   - Go to Actions tab
   - You should see "Create Release" workflow running
   - When done, check the "Releases" section of your repo

**🎉 CONGRATULATIONS!** You just automated releases!

---

## Part 6: Best Practices

### Workflow Organization

**✅ DO:**
- Keep workflows focused (one job per workflow if possible)
- Use descriptive names
- Add comments for complex steps
- Test workflows on feature branches first

**❌ DON'T:**
- Put everything in one massive workflow
- Use hardcoded secrets (use GitHub Secrets instead)
- Skip error handling

### Security

**🔒 IMPORTANT:**

Never commit secrets (passwords, API keys) in workflow files!

Instead, use GitHub Secrets:
1. Go to repository Settings → Secrets and variables → Actions
2. Click "New repository secret"
3. Add your secret
4. Use it in workflows: `${{ secrets.MY_SECRET }}`

---

## Part 7: Troubleshooting

### Common Problems

**Problem: Workflow doesn't run**
- ✅ Check: Is the file in `.github/workflows/`?
- ✅ Check: Is the file named `.yml` or `.yaml`?
- ✅ Check: Is the syntax correct (indentation matters in YAML)?

**Problem: Build fails**
- ✅ Check: Error message in Actions tab
- ✅ Check: Does it build locally? (`dotnet build`)
- ✅ Check: Are all dependencies in `.csproj`?

**Problem: Can't find artifacts**
- ✅ Check: Did the upload step run?
- ✅ Check: Is the path correct?
- ✅ Check: Artifacts expire after retention period

### Getting Help

1. **Check the logs** - Click on failed step to see error
2. **Test locally** - Run the same commands on your machine
3. **GitHub Actions docs** - https://docs.github.com/en/actions
4. **Community** - Stack Overflow, GitHub Discussions

---

## Part 8: Next Steps

### What to Add Next:

1. **Code Quality Checks**
   - Linting (StyleCop, Roslyn analyzers)
   - Code coverage reports

2. **Automated Testing**
   - Unit tests
   - Integration tests
   - Test coverage reporting

3. **Deployment**
   - Auto-deploy to staging
   - Auto-deploy to production (with approval)

4. **Notifications**
   - Slack/Discord notifications on failures
   - Email notifications

### Practice Exercises:

**Exercise 1:** Add a workflow that runs on a schedule (daily at midnight)
```yaml
on:
  schedule:
    - cron: '0 0 * * *'  # Runs daily at midnight UTC
```

**Exercise 2:** Add a workflow that only runs on specific file changes
```yaml
on:
  push:
    paths:
      - 'Models/**'
      - 'Services/**'
```

**Exercise 3:** Add a workflow that comments on PRs with build status

---

## Summary

You've learned:
- ✅ How to create GitHub Actions workflows
- ✅ How to build .NET projects automatically
- ✅ How to create releases automatically
- ✅ How to troubleshoot common issues

**Remember:** The best way to learn CI/CD is by doing. Experiment, break things, fix them, and learn!

---

## Quick Reference

### Workflow File Structure
```
.github/
  workflows/
    build.yml      # Builds on every push
    release.yml    # Creates releases on tags
    test.yml       # Runs tests
```

### Common Actions
- `actions/checkout@v4` - Get your code
- `actions/setup-dotnet@v4` - Install .NET
- `actions/upload-artifact@v4` - Save build files
- `softprops/action-gh-release@v1` - Create releases

### Useful Commands
```bash
# Test workflow locally (requires act tool)
act -j build

# Check workflow syntax
# GitHub validates automatically, but you can use:
yamllint .github/workflows/*.yml
```

---

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [.NET GitHub Actions](https://github.com/actions/setup-dotnet)
- [Workflow Syntax](https://docs.github.com/en/actions/using-workflows/workflow-syntax-for-github-actions)
- [Marketplace](https://github.com/marketplace?type=actions) - Find pre-built actions

---

**🎓 You're now ready to set up CI/CD! Start with Part 2 and work through each step. Good luck!**

