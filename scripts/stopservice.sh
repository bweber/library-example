#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/..

docker stop library-api

result=$?

cd "$current_directory"

exit $result