#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Inventory/ShopDev.Inventory.API/ShopDev.Inventory.API.csproj", "Services/Inventory/ShopDev.Inventory.API/"]
COPY ["Library/ShopDev.IdentityServerBase/ShopDev.IdentityServerBase.csproj", "Library/ShopDev.IdentityServerBase/"]
COPY ["Library/ShopDev.ApplicationBase/ShopDev.ApplicationBase.csproj", "Library/ShopDev.ApplicationBase/"]
COPY ["Library/ShopDev.EntitiesBase/ShopDev.EntitiesBase.csproj", "Library/ShopDev.EntitiesBase/"]
COPY ["Library/ShopDev.Utils/ShopDev.Utils.csproj", "Library/ShopDev.Utils/"]
COPY ["Services/Shared/ShopDev.Constants/ShopDev.Constants.csproj", "Services/Shared/ShopDev.Constants/"]
COPY ["Library/ShopDev.RabbitMQ/ShopDev.RabbitMQ.csproj", "Library/ShopDev.RabbitMQ/"]
COPY ["Library/ShopDev.ServiceDiscovery/ShopDev.ServiceDiscovery.csproj", "Library/ShopDev.ServiceDiscovery/"]
COPY ["Library/ShopDev.WebAPIBase/ShopDev.WebAPIBase.csproj", "Library/ShopDev.WebAPIBase/"]
COPY ["Library/ShopDev.ConvertFile/ShopDev.ConvertFile.csproj", "Library/ShopDev.ConvertFile/"]
COPY ["Library/ShopDev.InfrastructureBase/ShopDev.InfrastructureBase.csproj", "Library/ShopDev.InfrastructureBase/"]
COPY ["Library/ShopDev.S3Bucket/ShopDev.S3Bucket.csproj", "Library/ShopDev.S3Bucket/"]
COPY ["Services/Shared/ShopDev.Common/ShopDev.Common.csproj", "Services/Shared/ShopDev.Common/"]
COPY ["Services/Authentication/ShopDev.Authentication.Infrastructure/ShopDev.Authentication.Infrastructure.csproj", "Services/Authentication/ShopDev.Authentication.Infrastructure/"]
COPY ["Services/Authentication/ShopDev.Authentication.Domain/ShopDev.Authentication.Domain.csproj", "Services/Authentication/ShopDev.Authentication.Domain/"]
COPY ["Services/Shared/ShopDev.UserRolePermission/ShopDev.UserRolePermission.csproj", "Services/Shared/ShopDev.UserRolePermission/"]
COPY ["Services/Authentication/ShopDev.Authentication.ApplicationServices/ShopDev.Authentication.ApplicationServices.csproj", "Services/Authentication/ShopDev.Authentication.ApplicationServices/"]
COPY ["Library/ShopDev.Notification/ShopDev.Notification.csproj", "Library/ShopDev.Notification/"]
COPY ["Services/Inventory/ShopDev.Inventory.ApplicationServices/ShopDev.Inventory.ApplicationServices.csproj", "Services/Inventory/ShopDev.Inventory.ApplicationServices/"]
COPY ["Services/Inventory/ShopDev.Inventory.Domain/ShopDev.Inventory.Domain.csproj", "Services/Inventory/ShopDev.Inventory.Domain/"]
COPY ["Services/Inventory/ShopDev.Inventory.Infrastructure/ShopDev.Inventory.Infrastructure.csproj", "Services/Inventory/ShopDev.Inventory.Infrastructure/"]
RUN dotnet restore "./Services/Inventory/ShopDev.Inventory.API/ShopDev.Inventory.API.csproj"
COPY . .
WORKDIR "/src/Services/Inventory/ShopDev.Inventory.API"
RUN dotnet build "./ShopDev.Inventory.API.csproj" -c $BUILD_CONFIGURATION -p:RunAnalyzers=false -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ShopDev.Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ENV TZ=Asia/Ho_Chi_Minh
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShopDev.Inventory.API.dll"]
