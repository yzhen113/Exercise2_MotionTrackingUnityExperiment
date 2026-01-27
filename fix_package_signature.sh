#!/bin/bash

# Unity Package Signature Fix Script
# This script fixes invalid package signature errors by clearing corrupted cache

echo "Unity Package Signature Fix Script"
echo "==================================="
echo ""

# Check if Unity is running
if pgrep -x "Unity" > /dev/null; then
    echo "ERROR: Unity is currently running!"
    echo "Please close Unity before running this script."
    exit 1
fi

# Navigate to project directory
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

echo "Project directory: $SCRIPT_DIR"
echo ""

# Delete packages-lock.json
if [ -f "Packages/packages-lock.json" ]; then
    echo "Deleting packages-lock.json..."
    rm "Packages/packages-lock.json"
    echo "✓ Deleted packages-lock.json"
else
    echo "⚠ packages-lock.json not found"
fi

# Delete PackageCache
if [ -d "Library/PackageCache" ]; then
    echo "Deleting Library/PackageCache..."
    rm -rf "Library/PackageCache"
    echo "✓ Deleted PackageCache (packages will be re-downloaded)"
else
    echo "⚠ PackageCache not found"
fi

# Delete Temp folder (optional but recommended)
if [ -d "Temp" ]; then
    echo "Deleting Temp folder..."
    rm -rf "Temp"
    echo "✓ Deleted Temp folder"
fi

echo ""
echo "=========================================="
echo "Fix complete!"
echo ""
echo "Next steps:"
echo "1. Open Unity"
echo "2. Wait for packages to re-download (this may take several minutes)"
echo "3. Check Package Manager - signature errors should be resolved"
echo ""
echo "Note: All packages will be re-downloaded with fresh signatures"
echo "=========================================="
