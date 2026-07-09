# Build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.sln .
COPY Api/*.csproj Api/
COPY Domain/*.csproj Domain/
COPY Infrastructure/*.csproj Infrastructure/
COPY Application/*.csproj Application/

RUN dotnet restore

COPY . .

WORKDIR /src/Api
RUN dotnet publish -c Release -o /app/publish

# Run

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libgssapi-krb5-2 fonts-liberation \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "Api.dll"]
