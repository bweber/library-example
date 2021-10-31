#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/../Library.Infrastructure

if [[ -z "${AZURE_CREDENTIALS}" ]]; then
    echo "AZURE_CREDENTIALS environment variable is not set"
    exit 1
fi

export ARM_CLIENT_ID=$(echo $AZURE_CREDENTIALS | python3 -c "import sys, json; print(json.load(sys.stdin)['clientId'])")
export ARM_CLIENT_SECRET=$(echo $AZURE_CREDENTIALS | python3 -c "import sys, json; print(json.load(sys.stdin)['clientSecret'])")
export ARM_SUBSCRIPTION_ID=$(echo $AZURE_CREDENTIALS | python3 -c "import sys, json; print(json.load(sys.stdin)['subscriptionId'])")
export ARM_TENANT_ID=$(echo $AZURE_CREDENTIALS | python3 -c "import sys, json; print(json.load(sys.stdin)['tenantId'])")

pulumi login
(pulumi stack init $PULUMI_STACK) || echo "Pulumi $PULUMI_STACK already exists"
pulumi stack select $PULUMI_STACK

# Examples of setting configuration from CLI
pulumi config set myConfig "My Config"
pulumi config set --secret mySecret "My Secret"

if [ "$IS_PR_WORKFLOW" = true ] ; then
  pulumi preview
else
  pulumi up -y
fi

result=$?

cd "$current_directory"

exit $result
