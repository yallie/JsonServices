@echo off

dotnet test JsonServices.Core.Tests
dotnet test JsonServices.Serialization.Tests
dotnet test JsonServices.Transport.Fleck.Tests
dotnet test JsonServices.Transport.NetMQ.Tests
dotnet test JsonServices.Transport.WebSocketSharp.Tests
