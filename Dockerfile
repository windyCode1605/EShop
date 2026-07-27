# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["eShop.API/eShop.API.csproj", "eShop.API/"]
RUN dotnet restore "eShop.API/eShop.API.csproj"
COPY . .
WORKDIR "/src/eShop.API"
RUN dotnet publish "eShop.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "eShop.API.dll"]
