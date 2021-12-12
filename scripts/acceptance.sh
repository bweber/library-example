#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/..

echo "Running acceptance tests..."

export GIT_SHA=$(git rev-parse HEAD)

dotnet restore -v=quiet
dotnet build Library.AcceptanceTests/Library.AcceptanceTests.csproj --no-restore -c Release -v=quiet
dotnet test Library.AcceptanceTests/Library.AcceptanceTests.csproj --no-build -c Release -v=normal --logger "console;verbosity=detailed"

result=$?

cd "$current_directory"

exit $result
