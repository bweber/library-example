MAKEFLAGS += --silent

verify:
	docker-compose pull && ./scripts/verify.sh
	
start-local:
	docker-compose pull && ./scripts/startlocal.sh

build:
	./scripts/build.sh
	
acceptance:
	./scripts/acceptance.sh
	
integration:
	./scripts/integration.sh

start-docker:
	./scripts/startdocker.sh
	
stop-docker:
	./scripts/stopdocker.sh

stop-service:
	./scripts/stopservice.sh
	
setup-infra:
	./scripts/setupinfra.sh
	
migrations-add:
	dotnet ef \
		--project=Library.Common \
		--startup-project=Library.API \
		migrations add $(NAME) \
		--context Library.Common.Data.LibraryDBContext \
		--output-dir=Data/Migrations

database-update:
	dotnet ef \
		--project=Library.Common \
		--startup-project=Library.API \
		database update \
		--context Library.Common.Data.LibraryDBContextt