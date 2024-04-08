#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mShopDev.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mShopDev.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Authentication/ShopDev.Authentication.API/ShopDev.Authentication.API.csproj", "Services/Authentication/ShopDev.Authentication.API/"]
COPY ["Library/ShopDev.IdentityServerBase/ShopDev.IdentityServerBase.csproj", "Library/ShopDev.IdentityServerBase/"]
COPY ["Library/ShopDev.ApplicationBase/ShopDev.ApplicationBase.csproj", "Library/ShopDev.ApplicationBase/"]
COPY ["Library/ShopDev.Utils/ShopDev.Utils.csproj", "Library/ShopDev.Utils/"]
COPY ["Services/Shared/ShopDev.Constants/ShopDev.Constants.csproj", "Services/Shared/ShopDev.Constants/"]
COPY ["Library/ShopDev.EntitiesBase/ShopDev.EntitiesBase.csproj", "Library/ShopDev.EntitiesBase/"]
COPY ["Library/ShopDev.WebAPIBase/ShopDev.WebAPIBase.csproj", "Library/ShopDev.WebAPIBase/"]
COPY ["Services/Shared/ShopDev.Common/ShopDev.Common.csproj", "Services/Shared/ShopDev.Common/"]
COPY ["Library/ShopDev.InfrastructureBase/ShopDev.InfrastructureBase.csproj", "Library/ShopDev.InfrastructureBase/"]
COPY ["Library/Infrastructure/ShopDev.ConvertFile/ShopDev.ConvertFile.csproj", "Library/Infrastructure/ShopDev.ConvertFile/"]
COPY ["Library/Infrastructure/ShopDev.RabbitMQ/ShopDev.RabbitMQ.csproj", "Library/Infrastructure/ShopDev.RabbitMQ/"]
COPY ["Library/Infrastructure/ShopDev.S3Bucket/ShopDev.S3Bucket.csproj", "Library/Infrastructure/ShopDev.S3Bucket/"]
COPY ["Services/Shared/ShopDev.UserRolePermission/ShopDev.UserRolePermission.csproj", "Services/Shared/ShopDev.UserRolePermission/"]
COPY ["Services/Authentication/ShopDev.Authentication.ApplicationServices/ShopDev.Authentication.ApplicationServices.csproj", "Services/Authentication/ShopDev.Authentication.ApplicationServices/"]
COPY ["Library/Infrastructure/ShopDev.Notification/ShopDev.Notification.csproj", "Library/Infrastructure/ShopDev.Notification/"]
COPY ["Services/Authentication/ShopDev.Authentication.Domain/ShopDev.Authentication.Domain.csproj", "Services/Authentication/ShopDev.Authentication.Domain/"]
COPY ["Services/Authentication/ShopDev.Authentication.Infrastructure/ShopDev.Authentication.Infrastructure.csproj", "Services/Authentication/ShopDev.Authentication.Infrastructure/"]
COPY ["Services/Core/ShopDev.Core.Domain/ShopDev.Core.Domain.csproj", "Services/Core/ShopDev.Core.Domain/"]
RUN dotnet restore "./Services/Authentication/ShopDev.Authentication.API/./ShopDev.Authentication.API.csproj"
COPY . .
WORKDIR "/src/Services/Authentication/ShopDev.Authentication.API"
RUN dotnet build "./ShopDev.Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShopDev.Authentication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShopDev.Authentication.API.dll"]