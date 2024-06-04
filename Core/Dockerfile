#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Core/Core.csproj", "Core/"]
COPY ["Modules.CustomerManagement.Api/Modules.CustomerManagement.Api.csproj", "Modules.CustomerManagement.Api/"]
COPY ["Modules.CustomerManagement.Application/Modules.CustomerManagement.Application.csproj", "Modules.CustomerManagement.Application/"]
COPY ["Modules.CustomerManagement.Domain/Modules.CustomerManagement.Domain.csproj", "Modules.CustomerManagement.Domain/"]
COPY ["Shared.Utilities/Shared.Utilities.Models.csproj", "Shared.Utilities/"]
COPY ["Shared.Infrastructre/Shared.Infrastructre.csproj", "Shared.Infrastructre/"]
COPY ["Modules.OrderManagement.Domain/Modules.OrderManagement.Domain.csproj", "Modules.OrderManagement.Domain/"]
COPY ["Modules.ProductCatalog.Domain/Modules.ProductCatalog.Domain.csproj", "Modules.ProductCatalog.Domain/"]
COPY ["Modules.ShoppingCart.Domain/Modules.ShoppingCart.Domain.csproj", "Modules.ShoppingCart.Domain/"]
COPY ["Modules.PaymentProcessing.Domain/Modules.PaymentProcessing.Domain.csproj", "Modules.PaymentProcessing.Domain/"]
COPY ["Modules.OrderManagement.Api/Modules.OrderManagement.Api.csproj", "Modules.OrderManagement.Api/"]
COPY ["Modules.OrderManagement.Application/Modules.OrderManagement.Application.csproj", "Modules.OrderManagement.Application/"]
COPY ["Modules.PaymentProcessing.Application/Modules.PaymentProcessing.Application.csproj", "Modules.PaymentProcessing.Application/"]
COPY ["Utilities.Shared.Services/Utilities.Shared.Services.csproj", "Utilities.Shared.Services/"]
COPY ["Modules.PaymentProcessing.Api/Modules.PaymentProcessing.Api.csproj", "Modules.PaymentProcessing.Api/"]
COPY ["Modules.ProductCatalog.Api/Modules.ProductCatalog.Api.csproj", "Modules.ProductCatalog.Api/"]
COPY ["Modules.ProductCatalog.Application/Modules.ProductCatalog.Application.csproj", "Modules.ProductCatalog.Application/"]
COPY ["Modules.ShoppingCart.Api/Modules.ShoppingCart.Api.csproj", "Modules.ShoppingCart.Api/"]
COPY ["Modules.ShoppingCart.Application/Modules.ShoppingCart.Application.csproj", "Modules.ShoppingCart.Application/"]
RUN dotnet restore "./Core/./Core.csproj"
COPY . .
WORKDIR "/src/Core"
RUN dotnet build "./Core.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Core.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Core.dll"]