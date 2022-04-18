#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["E-Journal.WebUI/E-Journal.WebUI.csproj", "E-Journal.WebUI/"]
RUN dotnet restore "E-Journal.WebUI/E-Journal.WebUI.csproj"
COPY . .
WORKDIR "/src/E-Journal.WebUI"
RUN dotnet build "E-Journal.WebUI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "E-Journal.WebUI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E-Journal.WebUI.dll"]