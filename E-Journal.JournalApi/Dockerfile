#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["E-Journal.JournalApi/E-Journal.JournalApi.csproj", "E-Journal.JournalApi/"]
RUN dotnet restore "E-Journal.JournalApi/E-Journal.JournalApi.csproj"
COPY . .
WORKDIR "/src/E-Journal.JournalApi"
RUN dotnet build "E-Journal.JournalApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "E-Journal.JournalApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E-Journal.JournalApi.dll"]