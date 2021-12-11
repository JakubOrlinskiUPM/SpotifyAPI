FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SpotifyAPI.API/SpotifyAPI.API.csproj", "SpotifyAPI.API/"]
RUN dotnet restore "SpotifyAPI.API/SpotifyAPI.API.csproj"
COPY . .
WORKDIR "/src/SpotifyAPI.API"
RUN dotnet build "SpotifyAPI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SpotifyAPI.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SpotifyAPI.API.dll"]
