docker pull mcr.microsoft.com/mssql/server:2022-latest
docker stop sql1
docker rm sql1
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong@Passw0rd!" \
   -v ./docker/setup.sql:/setup.sql \
   -p 1433:1433 --name sql1 --hostname sql1 \
   -d \
   mcr.microsoft.com/mssql/server:2022-latest
# Wait 30 seconds for SQL Server to start up by ensuring that
sleep 30
docker exec sql1 /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P "YourStrong@Passw0rd!" -i ./setup.sql;

