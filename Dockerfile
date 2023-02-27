#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Trading.Api/Trading.Api.csproj", "Trading.Api/"]
COPY ["Trading.BL/Trading.BL.csproj", "Trading.BL/"]
COPY ["Trading.Common/Trading.Common.csproj", "Trading.Common/"]
RUN dotnet restore "Trading.Api/Trading.Api.csproj"
COPY . .
WORKDIR "/src/Trading.Api"
RUN dotnet build "Trading.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Trading.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Trading.Api.dll"]