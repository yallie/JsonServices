@echo off

:: start server
start dotnet run --project sample-server --framework net46 -c Debug
if not "%1" == "" goto :skip_client

:: start client
pushd sample-client
yarn
yarn start
popd

:skip_client