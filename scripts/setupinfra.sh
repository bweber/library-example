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

if [[ -z "${ARM_CLIENT_ID}" ]]; then
    echo "clientId could not be parsed from AZURE_CREDENTIALS"
    exit 1
fi

if [[ -z "${ARM_CLIENT_SECRET}" ]]; then
    echo "clientSecret could not be parsed from AZURE_CREDENTIALS"
    exit 1
fi

if [[ -z "${ARM_SUBSCRIPTION_ID}" ]]; then
    echo "subscriptionId could not be parsed from AZURE_CREDENTIALS"
    exit 1
fi

if [[ -z "${ARM_TENANT_ID}" ]]; then
    echo "tenantId could not be parsed from AZURE_CREDENTIALS"
    exit 1
fi

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
  
  APP_SERVICE_NAME=$(pulumi stack output appServiceName)
  APP_SERVICE_URL=$(pulumi stack output appServiceUrl)
  
  echo $APP_SERVICE_NAME
  echo $APP_SERVICE_URL
  
  echo "::set-output name=appServiceName::$APP_SERVICE_NAME"
  echo "::set-output name=appServiceUrl::$APP_SERVICE_URL"
fi

result=$?

cd "$current_directory"

exit $result