FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./src/Bot.Api/Bot.Api.csproj", "src/Bot.Api/"]
COPY ["./src/Bot.Api/appsettings.json", "src/Bot.Api/"]
COPY ["./src/Domain/Domain.csproj", "src/Domain/"]
COPY ["./src/Application/Application.csproj", "src/Application/"]
COPY ["./src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
RUN dotnet restore "src/Bot.Api/Bot.Api.csproj"
WORKDIR "src/Bot.Api"
COPY . .
RUN dotnet build "Bot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bot.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bot.Api.dll"]