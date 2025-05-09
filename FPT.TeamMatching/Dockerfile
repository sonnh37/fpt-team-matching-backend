﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FPT.TeamMatching.sln", "."]
COPY ["FPT.TeamMatching.API/FPT.TeamMatching.API.csproj", "FPT.TeamMatching.API/"]
COPY ["FPT.TeamMatching.Data/FPT.TeamMatching.Data.csproj", "FPT.TeamMatching.Data/"]
COPY ["FPT.TeamMatching.Domain/FPT.TeamMatching.Domain.csproj", "FPT.TeamMatching.Domain/"]
COPY ["FPT.TeamMatching.Services/FPT.TeamMatching.Services.csproj", "FPT.TeamMatching.Services/"]
RUN dotnet restore "FPT.TeamMatching.sln"

#RUN #dotnet restore "FPT.TeamMatching.API/FPT.TeamMatching.API.csproj"
COPY . .
WORKDIR "/src/FPT.TeamMatching.API"
RUN dotnet build "FPT.TeamMatching.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FPT.TeamMatching.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER root
RUN chmod -R 755 /app
USER $APP_UID
ENTRYPOINT ["dotnet", "FPT.TeamMatching.API.dll"]
