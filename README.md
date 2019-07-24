# JsonServices

[![Appveyor build status](https://ci.appveyor.com/api/projects/status/l8sntux7xbx53rk6?svg=true)](https://ci.appveyor.com/project/yallie/jsonservices)
[![Tests](https://img.shields.io/appveyor/tests/yallie/JsonServices.svg)](https://ci.appveyor.com/project/yallie/JsonServices/build/tests)
[![Code coverage](https://codecov.io/gh/yallie/JsonServices/branch/master/graph/badge.svg)](https://codecov.io/gh/yallie/JsonServices)
[![Nuget](https://img.shields.io/nuget/vpre/JsonServices.svg)](https://www.nuget.org/packages/JsonServices/)

This is a simple library for message-based services running on top of the  
WebSockets or ZeroMQ connection and based on JSON-RPC 2.0 Specification:  

https://www.jsonrpc.org/specification

The project consists of C# server, C# client and TypeScript client.  
Note that TypeScript client supports only web socket connections.

## Message-based RPC

The communication is based on message names, not service and/or method names  
like normal RPC. The idea is inspired by ServiceStack architecture.

C# server code example:

```c#
// request message
public class GetUser : IReturn<GetUserResponse>
{
  public long? UserId { get; set; }
  public string UserName { get; set; }
}

// response message
public class GetUserResponse
{
  public long UserId { get; set; }
  public string UserName { get; set; }
  public string Email { get; set; }
}

// service handler
public class GetUserService: IService<GetUser>
{
  public GetUserResponse Execute(GetUser request)
  {
    if (request.UserId.HasValue)
    {
      return GetUserById(request.UserId.Value);
    }

    return GetUserByName(request.UserName);
  }
	...
}
```

TypeScript client code example:

```typescript
// request message
public class GetUser implements IReturn<GetUserResponse> {
  public userId?: number;
  public userName?: string;

  public createResponse() {
    return new GetUserResponse();
  }
}

// response message
public class GetUserResponse {
  userId: number;
  userName: string;
  email: string;
}

// client code
const client = new JsonClient("ws://localhost:8765/");

const getUser = new GetUser();
getUser.userId = 7;

const result = await client.call(getUser);
```

## JSON-RPC messages for the above example

Normal method execution:

```
→ { "jsonrpc": "2.0", "method": "GetUser", "params": { "UserID": 1 }, "id": 1 }
← { "jsonrpc": "2.0", "result": { "UserID": 1, "UserName": "root", "Email": "noreply@example.com" }, "id": 1 }
```

Server reply when the method is not found:

```
← { "jsonrpc": "2.0", "error": { "code": -32601, "message": "Method not found"}, "id": "1" }
```

Event subscription (one-way call):

```
→ { "jsonrpc": "2.0", "method": "rpc.subscribe", "params": [ "FeedUpdated", "MessageSent" ] }
```

Event unsubscription (one-way call):

```
→ { "jsonrpc": "2.0", "method": "rpc.unsubscribe", "params": [ "MessageSent" ] }
```

Server-side notification:

```
← { "jsonrpc": "2.0", "method": "MessageSent", params: { "text": "Hello world!" } }
```

## Transport-specific notes

### WebSockets (via WebSocketSharp)

* Implements both client and server
* Supports both IP addresses and host names, i.e. ws://localhost:8765
* .NET 4.5 only
* Chrome browser's WebSocket implementation connects to WebSocketSharp server
* Node.js ws module seem to have troubles connecting to WebSocketSharp server

### WebSockets (via Fleck)

* Implements only server (WebSocketSharp client can connect to Flex server)
* Supports only IP addresses, i.e. ws://127.0.0.1:8765
* .NET 4.5 and .NET Standard 2.0
* Chrome browser's WebSocket implementation connects to Fleck server
* Node.js ws module also connects to Fleck server

### ZeroMQ (via NetMQ)

* Implements both client and server
* Supports only tcp protocol
* .NET 4.5 and .NET Standard 2.0
* Browsers are unable to connect to ZeroMQ endpoints
* Node.js in theory should be able to connect using zeromq.js npm (not tested)
