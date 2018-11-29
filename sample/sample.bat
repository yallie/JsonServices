@echo off

:: start server
start dotnet run --project sample-server --framework net45
if not "%1" == "" goto :skip_client

:: start client
pushd sample-client
yarn start
popd

:skip_client