#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/..

echo "Running acceptance tests..."

export GIT_HASH=$(git rev-parse HEAD)

dotnet test Library.AcceptanceTests/Library.AcceptanceTests.csproj --no-restore --no-build -c Release -v=normal

result=$?

cd "$current_directory"

exit $result