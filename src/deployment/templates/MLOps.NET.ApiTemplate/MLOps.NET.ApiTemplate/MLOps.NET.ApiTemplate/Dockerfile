#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MLOps.NET.ApiTemplate/MLOps.NET.ApiTemplate.csproj", "MLOps.NET.ApiTemplate/"]
RUN dotnet restore "MLOps.NET.ApiTemplate/MLOps.NET.ApiTemplate.csproj"
COPY . .
WORKDIR "/src/MLOps.NET.ApiTemplate"
RUN dotnet build "MLOps.NET.ApiTemplate.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MLOps.NET.ApiTemplate.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MLOps.NET.ApiTemplate.dll"]