FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TaskManagerPro.csproj", "./"]
RUN dotnet restore "TaskManagerPro.csproj"
COPY . .
WORKDIR "/src/"
COPY ["src/TaskMasterPro.API/TaskMasterPro.API.csproj", "TaskMasterPro.API/"]
COPY ["src/TaskMasterPro.Application/TaskMasterPro.Application.csproj", "TaskMasterPro.Application/"]
COPY ["src/TaskMasterPro.Domain/TaskMasterPro.Domain.csproj", "TaskMasterPro.Domain/"]
COPY ["src/TaskMasterPro.Infrastructure/TaskMasterPro.Infrastructure.csproj", "TaskMasterPro.Infrastructure/"]

RUN dotnet restore "TaskMasterPro.API/TaskMasterPro.API.csproj"
COPY . .
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TaskManagerPro.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskManagerPro.dll"]
