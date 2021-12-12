#!/bin/bash
set -e

if [[ -z "${GIT_SHA}" ]]; then
    echo "GIT_SHA environment variable is not set"
    exit 1
fi

current_directory="$PWD"

cd $(dirname $0)/..

echo "Running integration tests..."

dotnet restore -v=quiet
dotnet build Library.IntegrationTests/Library.IntegrationTests.csproj --no-restore -c Release -v=minimal
dotnet test Library.IntegrationTests/Library.IntegrationTests.csproj --no-build --no-restore -c Release -v=normal --logger "console;verbosity=detailed" --filter $TEST_FILTER

result=$?

cd "$current_directory"

exit $result
