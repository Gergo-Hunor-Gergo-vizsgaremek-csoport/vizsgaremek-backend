#!/bin/sh
#dotnet tool install --global dotnet-ef
dotnet ef migrations add $1
