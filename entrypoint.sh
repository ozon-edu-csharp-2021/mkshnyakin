#!/bin/bash

set -e
run_cmd="dotnet OzonEdu.MerchandiseService.dll --no-build -v d"

>&2 echo "============ Dry run migrations ============"
dotnet OzonEdu.MerchandiseService.Migrator.dll --no-build -v d -- --dryrun

>&2 echo "============ Run migrations ============"
dotnet OzonEdu.MerchandiseService.Migrator.dll --no-build -v d

>&2 echo "MerchandiseService DB Migrations complete, starting app."

>&2 echo "============ Wait for Kafka ============"
BOOTSTRAP_SERVERS="${KafkaConfiguration__BootstrapServers:-localhost:9092}"
echo "$BOOTSTRAP_SERVERS";
./wait-for-it.sh "$BOOTSTRAP_SERVERS" -- ./create-kafka-topics.sh

>&2 echo "Run MerchandiseService: $run_cmd"
exec $run_cmd