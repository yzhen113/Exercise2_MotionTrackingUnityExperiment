# Troubleshooting: Assets Unresponsive on Another Computer

If assets become unresponsive when opening this Unity project on another computer, follow these steps:

## Quick Fix Steps

### 1. Delete Library Folder (Most Common Fix)
1. Close Unity completely
2. Navigate to the project folder
3. Delete the `Library` folder
4. Reopen Unity - it will regenerate the Library folder
5. Wait for Unity to reimport all assets (this may take several minutes)

### 2. Reimport All Assets
1. In Unity, go to: **Assets > Reimport All**
2. Wait for the import process to complete

### 3. Check Unity Version Compatibility
- This project uses Unity 6000.3.3f1 (Unity 6)
- Make sure the other computer has the same or compatible Unity version
- If using a different version, Unity will prompt to upgrade/downgrade

### 4. Verify Meta Files
- All assets should have corresponding `.meta` files
- If meta files are missing, Unity will regenerate them, but this may break references
- **Important**: Always commit `.meta` files to version control (they're in `.gitignore` exclusion)

### 5. Check Package Manager
1. Open **Window > Package Manager**
2. Check for any missing packages or errors
3. Click **Resolve** if there are dependency issues

### 6. Clear Cache (If Using Version Control)
If using Git/version control:
1. Close Unity
2. Delete `Library` folder
3. Delete `Temp` folder
4. Reopen Unity

### 7. Check Asset References
1. Open the Console window (Window > General > Console)
2. Look for any "Missing Reference" errors
3. Reassign broken references manually

### 8. Platform Settings
1. Go to **File > Build Settings**
2. Make sure the correct platform is selected
3. Click **Switch Platform** if needed

## Prevention Tips

1. **Always commit `.meta` files** - They're essential for asset references
2. **Use the same Unity version** across all computers
3. **Don't commit the Library folder** - It's auto-generated
4. **Use Unity Cloud Build or similar** for consistent builds
5. **Keep packages in sync** - Use `Packages/manifest.json`

## Common Error Messages and Solutions

### "Missing Script" on GameObjects
- Scripts may be in a different location
- Check that all scripts are in the `Assets/script/` folder
- Reassign scripts in the Inspector

### "Unassigned Reference" in Inspector
- Asset references may be broken
- Reassign them manually in the Inspector
- Check that all assets are in the correct folders

### Assets Show as "Unreadable" or Gray
- Usually means Unity hasn't finished importing
- Wait for import to complete
- If stuck, delete Library folder and restart

## If Nothing Works

1. Create a fresh Unity project
2. Copy only the `Assets` and `ProjectSettings` folders
3. Let Unity regenerate everything else
4. Reimport all assets

## Contact
If issues persist, check:
- Unity version compatibility
- Operating system differences (Windows vs Mac vs Linux)
- File path length limits (Windows has 260 character limit)
