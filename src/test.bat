@echo off

dotnet test JsonServices.Core.Tests
dotnet test JsonServices.Serialization.Tests
dotnet test JsonServices.Transport.WebSocketSharp.Tests
dotnet test JsonServices.Transport.Fleck.Tests
