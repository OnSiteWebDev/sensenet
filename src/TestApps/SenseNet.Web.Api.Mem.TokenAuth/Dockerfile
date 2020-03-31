#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["TestApps/SenseNet.Web.Api.Mem.Oidc/SenseNet.Web.Api.Mem.Oidc.csproj", "TestApps/SenseNet.Web.Api.Mem.Oidc/"]
COPY ["TestApps/SenseNet.Web.Api.Mem.Oidc/SenseNet.Web.Api.Mem.Oidc.sln", "TestApps/SenseNet.Web.Api.Mem.Oidc/"]
COPY ["BlobStorage/SenseNet.BlobStorage.csproj", "BlobStorage/"]
COPY ["Common/SenseNet.Common.csproj", "Common/"]
COPY ["ContentRepository/SenseNet.ContentRepository.csproj", "ContentRepository/"]
COPY ["ContentRepository.InMemory/SenseNet.ContentRepository.InMemory.csproj", "ContentRepository.InMemory/"]
COPY ["OData/SenseNet.OData.csproj", "OData/"]
COPY ["Search/SenseNet.Search.csproj", "Search/"]
COPY ["Services.Core/SenseNet.Services.Core.csproj", "Services.Core/"]
COPY ["Storage/SenseNet.Storage.csproj", "Storage/"]
RUN dotnet restore "TestApps/SenseNet.Web.Api.Mem.Oidc/SenseNet.Web.Api.Mem.Oidc.sln"
COPY . .
WORKDIR "/src/TestApps/SenseNet.Web.Api.Mem.Oidc"
RUN dotnet build "SenseNet.Web.Api.Mem.Oidc.sln" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SenseNet.Web.Api.Mem.Oidc.sln" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SenseNet.Web.Api.Mem.Oidc.dll"]