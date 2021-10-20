FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

COPY src/OzonEdu.MerchandiseService/*.csproj .
RUN dotnet restore

COPY src/OzonEdu.MerchandiseService/. .
RUN dotnet publish -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "OzonEdu.MerchandiseService.dll"]

FROM runtime AS final
WORKDIR /app
COPY --from=build /app .
