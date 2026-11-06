FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props EIM.sln ./
COPY src/EIM.Domain/EIM.Domain.csproj src/EIM.Domain/
COPY src/EIM.Application/EIM.Application.csproj src/EIM.Application/
COPY src/EIM.Infrastructure/EIM.Infrastructure.csproj src/EIM.Infrastructure/
COPY src/EIM.Api/EIM.Api.csproj src/EIM.Api/
RUN dotnet restore src/EIM.Api/EIM.Api.csproj
COPY src/ src/
RUN dotnet publish src/EIM.Api/EIM.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "EIM.Api.dll"]
