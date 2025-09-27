# STAGE 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copia csproj e ripristina dipendenze
COPY *.csproj .
RUN dotnet restore

# copia il resto e compila
COPY . .
RUN dotnet publish -c Release -o /app

# STAGE 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "PortfolioApi.dll"]
