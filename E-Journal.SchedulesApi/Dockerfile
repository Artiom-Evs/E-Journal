#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["E-Journal.SchedulesApi/E-Journal.SchedulesApi.csproj", "E-Journal.SchedulesApi/"]
RUN dotnet restore "E-Journal.SchedulesApi/E-Journal.SchedulesApi.csproj"
COPY . .
WORKDIR "/src/E-Journal.SchedulesApi"
RUN dotnet build "E-Journal.SchedulesApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "E-Journal.SchedulesApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E-Journal.SchedulesApi.dll"]