FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build0
WORKDIR /source/OzonEdu.MerchandiseService.Platform
COPY src/OzonEdu.MerchandiseService.Platform/. .

FROM build0 AS build1
WORKDIR /source/OzonEdu.MerchandiseService.HttpModels
COPY src/OzonEdu.MerchandiseService.HttpModels/. .

FROM build1 AS build2
WORKDIR /source/OzonEdu.MerchandiseService.Grpc
COPY src/OzonEdu.MerchandiseService.Grpc/. .

FROM build2 AS build3
WORKDIR /source/OzonEdu.MerchandiseService.Domain
COPY src/OzonEdu.MerchandiseService.Domain/. .

FROM build3 AS build4
WORKDIR /source/OzonEdu.MerchandiseService.Infrastructure
COPY src/OzonEdu.MerchandiseService.Infrastructure/. .

FROM build4 AS build5
WORKDIR /source/OzonEdu.MerchandiseService.Migrator
COPY src/OzonEdu.MerchandiseService.Migrator/. .

FROM build5 AS build6
WORKDIR /source/OzonEdu.StockApi.GrpcClient
COPY src/OzonEdu.StockApi.GrpcClient/. .

FROM build6 AS build7
WORKDIR /source/Confluent.Kafka.AdminClient
COPY src/Confluent.Kafka.AdminClient/. .

FROM build7 AS build8
WORKDIR /source/OzonEdu.MerchandiseService
COPY src/OzonEdu.MerchandiseService/. .

FROM build8 AS publish
RUN dotnet publish -c Release -o /app
COPY create-kafka-topics.sh /app/.
COPY entrypoint.sh /app/.
COPY wait-for-it.sh /app/.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS runtime
EXPOSE 5000
EXPOSE 5001
#ENTRYPOINT ["dotnet", "OzonEdu.MerchandiseService.dll"]

FROM runtime AS final
WORKDIR /app
COPY --from=publish /app .
RUN chmod +x entrypoint.sh
RUN chmod +x wait-for-it.sh
CMD /bin/bash entrypoint.sh
#CMD ["wait-for-it.sh", "db:5432", "--", "entrypoint.sh"]
