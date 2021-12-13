#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#COPY ["SpotifyAPI.API/SpotifyAPI.API.csproj", "SpotifyAPI.API/"]
#COPY ["SpotifyAPI.Core/SpotifyAPI.Core.csproj", "SpotifyAPI.Core/"]
#COPY . .
#RUN dotnet build --disable-parallel "SpotifyAPI.API/SpotifyAPI.API.csproj" -c Release -o /app/build.api
#
#FROM build AS publish
#RUN dotnet publish "SpotifyAPI.API/SpotifyAPI.API.csproj" -c Release -o /app/publish.api
#
#FROM base AS final
#COPY --from=publish /app/publish.api ./app
#ENTRYPOINT ["dotnet", "./app/SpotifyAPI.API.dll"]

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
COPY . .
RUN dotnet publish --configuration nuget.config "SpotifyAPI.API/SpotifyAPI.API.csproj" -c Release -o ./app
ENTRYPOINT ["dotnet", "./app/SpotifyAPI.API.dll"]
