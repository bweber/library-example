#!/bin/bash
set -e

current_directory="$PWD"

cd $(dirname $0)/../Library.Infrastructure

pulumi login
(pulumi stack init $PULUMI_STACK) || echo "Pulumi $PULUMI_STACK already exists"
pulumi stack select $PULUMI_STACK

if [ "$IS_PR_WORKFLOW" = true ] ; then
  pulumi preview
else
  pulumi up -y
  
  APP_SERVICE_NAME=$(pulumi stack output appServiceName)
  APP_SERVICE_URL=$(pulumi stack output appServiceUrl)
  
  echo '::set-output name=appServiceName::${APP_SERVICE_NAME}'
  echo '::set-output name=appServiceUrl::${APP_SERVICE_URL}'

  echo $API_URL
fi

result=$?

cd "$current_directory"

exit $result