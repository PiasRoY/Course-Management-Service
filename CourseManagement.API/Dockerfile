# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5901

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["CourseManagement.API/CourseManagement.API.csproj", "CourseManagement.API/"]
COPY ["CourseManagement.Business/CourseManagement.Business.csproj", "CourseManagement.Business/"]
COPY ["CourseManagement.Domain/CourseManagement.Domain.csproj", "CourseManagement.Domain/"]
COPY ["CourseManagement.Infrastructure/CourseManagement.Infrastructure.csproj", "CourseManagement.Infrastructure/"]

RUN dotnet restore "./CourseManagement.API/CourseManagement.API.csproj"

COPY . .
WORKDIR "/src/CourseManagement.API"
RUN dotnet build "./CourseManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CourseManagement.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CourseManagement.API.dll"]