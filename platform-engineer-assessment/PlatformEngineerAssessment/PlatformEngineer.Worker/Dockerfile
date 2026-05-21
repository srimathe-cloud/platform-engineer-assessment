FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY PlatformEngineer.Worker/PlatformEngineer.Worker.csproj PlatformEngineer.Worker/

RUN dotnet restore PlatformEngineer.Worker/PlatformEngineer.Worker.csproj

COPY . .

RUN dotnet publish PlatformEngineer.Worker/PlatformEngineer.Worker.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "PlatformEngineer.Worker.dll"]