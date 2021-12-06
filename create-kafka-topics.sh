#!/bin/bash

BOOTSTRAP_SERVERS="${KafkaConfiguration__BootstrapServers:-localhost:9092}"

>&2 echo "============ Create Kafka topics ============"
dotnet Confluent.Kafka.AdminClient.dll "$BOOTSTRAP_SERVERS" create-topic email_notification_event
dotnet Confluent.Kafka.AdminClient.dll "$BOOTSTRAP_SERVERS" create-topic employee_notification_event
dotnet Confluent.Kafka.AdminClient.dll "$BOOTSTRAP_SERVERS" create-topic stock_replenished_event
dotnet Confluent.Kafka.AdminClient.dll "$BOOTSTRAP_SERVERS" create-topic supply_ship_event
