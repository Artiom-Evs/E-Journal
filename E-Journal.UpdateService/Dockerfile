#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["E-Journal.UpdateService/E-Journal.UpdateService.csproj", "E-Journal.UpdateService/"]
RUN dotnet restore "E-Journal.UpdateService/E-Journal.UpdateService.csproj"
COPY . .
WORKDIR "/src/E-Journal.UpdateService"
RUN dotnet build "E-Journal.UpdateService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "E-Journal.UpdateService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E-Journal.UpdateService.dll"]