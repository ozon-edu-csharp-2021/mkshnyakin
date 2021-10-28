FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build1
WORKDIR /source/OzonEdu.MerchandiseService.HttpModels
COPY src/OzonEdu.MerchandiseService.HttpModels/. .

FROM build1 AS build2
WORKDIR /source/OzonEdu.MerchandiseService.Grpc
COPY src/OzonEdu.MerchandiseService.Grpc/. .

FROM build2 AS build3
WORKDIR /source/OzonEdu.MerchandiseService
COPY src/OzonEdu.MerchandiseService/. .

FROM build3 AS publish
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
EXPOSE 5000
EXPOSE 5001
ENTRYPOINT ["dotnet", "OzonEdu.MerchandiseService.dll"]

FROM runtime AS final
WORKDIR /app
COPY --from=publish /app .
