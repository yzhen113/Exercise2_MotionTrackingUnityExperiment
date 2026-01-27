#!/bin/bash

# Unity Package Fix Script
# This script fixes common Unity package errors by clearing cache files

echo "Unity Package Fix Script"
echo "========================"
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
    echo "⚠ packages-lock.json not found (may have been deleted already)"
fi

# Delete PackageCache
if [ -d "Library/PackageCache" ]; then
    echo "Deleting Library/PackageCache..."
    rm -rf "Library/PackageCache"
    echo "✓ Deleted PackageCache"
else
    echo "⚠ PackageCache not found (may have been deleted already)"
fi

echo ""
echo "=========================================="
echo "Fix complete!"
echo ""
echo "Next steps:"
echo "1. Open Unity"
echo "2. Wait for packages to resolve"
echo "3. Check Package Manager for any remaining issues"
echo "=========================================="
