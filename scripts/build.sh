#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/..

dotnet restore

git rev-parse HEAD > Library.API/version.txt

dotnet build Library.sln --no-restore -c Release

# Run unit tests
dotnet test Library.Infrastructure.Tests --no-build --no-restore -c Release -v=normal

dotnet publish Library.API/Library.API.csproj -c Release -o publish --no-restore --no-build

result=$?

cd "$current_directory"

exit $result