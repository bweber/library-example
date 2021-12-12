#syntax=docker/dockerfile:1.2

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS builder

WORKDIR /app

ARG GIT_SHA

COPY *.sln .
COPY */*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done

RUN dotnet restore

COPY . .

RUN echo $GIT_SHA > Library.API/version.txt

RUN dotnet publish Library.API/Library.API.csproj -c Release -o publish --no-restore

########################################

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

WORKDIR /app

RUN apk update && apk upgrade

RUN apk add --no-cache icu-libs tzdata

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=builder /app/publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "Library.API.dll"]
