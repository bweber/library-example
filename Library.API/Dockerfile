FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine

# tzdata = Timezones Data File
RUN apk add --no-cache icu-libs tzdata
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "Library.API.dll"]

