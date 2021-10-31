#!/bin/bash
set -e

if [[ -z "${GIT_HASH}" ]]; then
    echo "GIT_HASH environment variable is not set"
    exit 1
fi

current_directory="$PWD"

cd $(dirname $0)/..

echo "Running integration tests..."

dotnet test Library.IntegrationTests/Library.IntegrationTests.csproj -c Release -v=normal

result=$?

cd "$current_directory"

exit $result
