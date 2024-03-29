#!/bin/bash
set -e

# verify the software

current_directory="$PWD"

cd $(dirname $0)

time {
    ./dependencycheck.sh
    ./stopdocker.sh
    ./build.sh
    IS_PR_WORKFLOW=true ./startdocker.sh
    ./stopservice.sh
}

cd "$current_directory"
