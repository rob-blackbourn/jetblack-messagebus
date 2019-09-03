#!/bin/bash

cd ${JETBLACK_MESSAGEBUS_ROOT}/core/src/JetBlack.MessageBus.Distributor
dotnet run -- ${JETBLACK_MESSAGEBUS_ROOT}/examples/server-config/ssl/appsettings.json
