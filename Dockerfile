FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Directory.Build.props TLighthouse.sln ./
COPY src/TLighthouse.Domain/TLighthouse.Domain.csproj src/TLighthouse.Domain/
COPY src/TLighthouse.Application/TLighthouse.Application.csproj src/TLighthouse.Application/
COPY src/TLighthouse.Infrastructure/TLighthouse.Infrastructure.csproj src/TLighthouse.Infrastructure/
COPY src/TLighthouse.Api/TLighthouse.Api.csproj src/TLighthouse.Api/
RUN dotnet restore src/TLighthouse.Api/TLighthouse.Api.csproj
COPY src/ src/
RUN dotnet publish src/TLighthouse.Api/TLighthouse.Api.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "TLighthouse.Api.dll"]
