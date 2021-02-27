FROM mcr.microsoft.com/mssql/server:2019-latest

USER root

# Create a config directory
RUN mkdir -p /usr/config
WORKDIR /usr/config

# Bundle config source
COPY . /usr/config

# Grant permissions for to our scripts to be executable
RUN chmod +x /usr/config/entrypoint.sh
RUN chmod +x /usr/config/configure-db.sh

ENTRYPOINT ["./entrypoint.sh"]

# Tail the setup logs to trap the process
CMD ["tail -f /dev/null"]

HEALTHCHECK --interval=15s CMD /opt/mssql-tools/bin/sqlcmd -U $MSSQL_USER -P $MSSQL_PASSWORD -d $MSSQL_DB -Q "select 1" && grep -q "MSSQL CONFIG COMPLETE" ./config.log