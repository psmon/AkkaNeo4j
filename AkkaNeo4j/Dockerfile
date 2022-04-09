#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["AkkaNeo4j/AkkaNeo4j.csproj", "AkkaNeo4j/"]
RUN dotnet restore "AkkaNeo4j/AkkaNeo4j.csproj"
COPY . .
WORKDIR "/src/AkkaNeo4j"
RUN dotnet build "AkkaNeo4j.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AkkaNeo4j.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AkkaNeo4j.dll"]