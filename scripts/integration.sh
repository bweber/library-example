#!/bin/bash
set -e

if [[ -z "${GIT_HASH}" ]]; then
    echo "GIT_HASH environment variable is not set"
    exit 1
fi

if [[ -z "${LIBRARY_API_URL}" ]]; then
    echo "LIBRARY_API_URL environment variable is not set"
    exit 1
fi

current_directory="$PWD"

cd $(dirname $0)/..

echo "Running integration tests..."

dotnet test Library.IntegrationTests/Library.IntegrationTests.csproj -c Release --no-restore --no-build -v=normal

result=$?

cd "$current_directory"

exit $result