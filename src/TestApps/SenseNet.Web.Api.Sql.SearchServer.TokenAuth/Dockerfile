#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["TestApps/SenseNet.Web.Api.Sql.SearchServer.TokenAuth/SenseNet.Web.Api.Sql.SearchServer.TokenAuth.csproj", "TestApps/SenseNet.Web.Api.Sql.SearchServer.TokenAuth/"]
RUN dotnet restore "TestApps/SenseNet.Web.Api.Sql.SearchServer.TokenAuth/SenseNet.Web.Api.Sql.SearchServer.TokenAuth.csproj"
COPY . .
WORKDIR "/src/TestApps/SenseNet.Web.Api.Sql.SearchServer.TokenAuth"
RUN dotnet build "SenseNet.Web.Api.Sql.SearchServer.TokenAuth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SenseNet.Web.Api.Sql.SearchServer.TokenAuth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SenseNet.Web.Api.Sql.SearchServer.TokenAuth.dll"]