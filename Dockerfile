FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY azure-task-api.sln .
COPY src/AzureTaskApi/AzureTaskApi.csproj src/AzureTaskApi/
RUN dotnet restore src/AzureTaskApi/AzureTaskApi.csproj
COPY . .
RUN dotnet publish src/AzureTaskApi/AzureTaskApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "AzureTaskApi.dll"]
