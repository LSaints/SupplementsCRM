#!/bin/bash
set -e

echo "========================================"
echo "  CRM - Pre-commit Setup"
echo "========================================"

# ========================================
# Frontend (npm)
# ========================================
echo ""
echo "[1/4] Installing frontend dependencies..."
npm install

# ========================================
# Backend (.NET tools)
# ========================================
echo ""
echo "[2/4] Installing .NET tools..."

if ! dotnet tool list -g | grep -q "husky"; then
    dotnet tool install -g husky
else
    echo "  husky already installed"
fi

if ! dotnet tool list -g | grep -q "csharpier"; then
    dotnet tool install -g csharpier
else
    echo "  csharpier already installed"
fi

# ========================================
# Restore .NET tools (optional, for lockfile)
# ========================================
echo ""
echo "[3/4] Restoring .NET tools..."
dotnet tool restore 2>/dev/null || true

# ========================================
# Husky init
# ========================================
echo ""
echo "[4/4] Initializing husky..."
husky install

echo ""
echo "========================================"
echo "  Setup complete!"
echo "========================================"
echo ""
echo "Usage:"
echo "  git commit -m 'your message'"
echo ""
echo "Hooks configured:"
echo "  - pre-commit:  eslint --fix + vitest (frontend)"
echo "                 csharpier + build + test (backend)"
echo "  - pre-push:   dotnet test"
echo ""