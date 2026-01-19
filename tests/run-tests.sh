#!/bin/bash
# SyncNet API Test Suite
# Executes tests in recommended order

set -e  # Exit on first failure

echo "🧪 Running SyncNet API Test Suite..."
echo ""

# Test counters
total_tests=0
passed_tests=0
failed_tests=0

# Color codes
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to run a test file
run_test() {
    local test_file=$1
    local test_name=$(basename "$test_file")
    
    echo "Running $test_name..."
    
    if hurl --test "$test_file" > /dev/null 2>&1; then
        echo -e "${GREEN}✓${NC} $test_name passed"
        ((passed_tests++))
    else
        echo -e "${RED}✗${NC} $test_name failed"
        ((failed_tests++))
        # Show detailed output on failure
        echo ""
        hurl --test "$test_file"
        echo ""
    fi
    
    ((total_tests++))
    echo ""
}

# Navigate to project root
cd "$(dirname "$0")/.."

# Check if API is running
if ! curl -s http://localhost:5000/health > /dev/null 2>&1; then
    echo -e "${YELLOW}⚠${NC} Warning: API doesn't seem to be running on localhost:5000"
    echo "   Start it with: cd SyncNet.Api && dotnet run --urls http://localhost:5000"
    echo ""
fi

# Execute tests in recommended order
echo "Test execution order:"
echo "1. pull.hurl       (read-only tests)"
echo "2. push.hurl       (basic operations)"
echo "3. sync-flow.hurl  (complete workflow)"
echo "4. edge-cases.hurl (edge cases & error handling)"
echo ""
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""

run_test "tests/pull.hurl"
run_test "tests/push.hurl"
run_test "tests/sync-flow.hurl"
run_test "tests/edge-cases.hurl"

# Summary
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo ""
echo "Test Summary:"
echo "  Total:  $total_tests"
echo -e "  ${GREEN}Passed: $passed_tests${NC}"
if [ $failed_tests -gt 0 ]; then
    echo -e "  ${RED}Failed: $failed_tests${NC}"
    exit 1
else
    echo -e "  ${RED}Failed: $failed_tests${NC}"
fi
echo ""
echo -e "${GREEN}✓ All tests passed!${NC}"
