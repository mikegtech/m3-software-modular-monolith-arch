# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/API/M3.Net.Api/M3.Net.Api.csproj", "src/API/M3.Net.Api/"]
COPY ["src/Common/M3.Net.Common.Application/M3.Net.Common.Application.csproj", "src/Common/M3.Net.Common.Application/"]
COPY ["src/Common/M3.Net.Common.Domain/M3.Net.Common.Domain.csproj", "src/Common/M3.Net.Common.Domain/"]
COPY ["src/Common/M3.Net.Common.Infrastructure/M3.Net.Common.Infrastructure.csproj", "src/Common/M3.Net.Common.Infrastructure/"]
COPY ["src/Common/M3.Net.Common.Presentation/M3.Net.Common.Presentation.csproj", "src/Common/M3.Net.Common.Presentation/"]
COPY ["src/Modules/Transcripts/M3.Net.Modules.Transcripts.Application/M3.Net.Modules.Transcripts.Application.csproj", "src/Modules/Transcripts/M3.Net.Modules.Transcripts.Application/"]
COPY ["src/Modules/Transcripts/M3.Net.Modules.Transcripts.Domain/M3.Net.Modules.Transcripts.Domain.csproj", "src/Modules/Transcripts/M3.Net.Modules.Transcripts.Domain/"]
COPY ["src/Modules/Transcripts/M3.Net.Modules.Transcripts.Infrastructure/M3.Net.Modules.Transcripts.Infrastructure.csproj", "src/Modules/Transcripts/M3.Net.Modules.Transcripts.Infrastructure/"]
COPY ["src/Modules/Transcripts/M3.Net.Modules.Transcripts.IntegrationEvents/M3.Net.Modules.Transcripts.IntegrationEvents.csproj", "src/Modules/Transcripts/M3.Net.Modules.Transcripts.IntegrationEvents/"]
COPY ["src/Modules/Transcripts/M3.Net.Modules.Transcripts.Presentation/M3.Net.Modules.Transcripts.Presentation.csproj", "src/Modules/Transcripts/M3.Net.Modules.Transcripts.Presentation/"]
COPY ["src/Modules/Users/M3.Net.Modules.Users.Application/M3.Net.Modules.Users.Application.csproj", "src/Modules/Users/M3.Net.Modules.Users.Application/"]
COPY ["src/Modules/Users/M3.Net.Modules.Users.Domain/M3.Net.Modules.Users.Domain.csproj", "src/Modules/Users/M3.Net.Modules.Users.Domain/"]
COPY ["src/Modules/Users/M3.Net.Modules.Users.Infrastructure/M3.Net.Modules.Users.Infrastructure.csproj", "src/Modules/Users/M3.Net.Modules.Users.Infrastructure/"]
COPY ["src/Modules/Users/M3.Net.Modules.Users.IntegrationEvents/M3.Net.Modules.Users.IntegrationEvents.csproj", "src/Modules/Users/M3.Net.Modules.Users.IntegrationEvents/"]
COPY ["src/Modules/Users/M3.Net.Modules.Users.Presentation/M3.Net.Modules.Users.Presentation.csproj", "src/Modules/Users/M3.Net.Modules.Users.Presentation/"]
COPY ["Directory.Build.props", "."]
COPY ["Directory.Packages.props", "."]
RUN dotnet restore "src/API/M3.Net.Api/M3.Net.Api.csproj"
COPY . .
WORKDIR "/src/src/API/M3.Net.Api"
RUN dotnet build "M3.Net.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "M3.Net.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "M3.Net.Api.dll"]