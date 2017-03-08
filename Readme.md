TL;DR File [MyCommunication/WcfWrapper.cs](https://github.com/ikriv/AspNetCallsWinService/blob/remoting/MyCommunication/RemotingWrapper.cs) contains C# code for creating .NET Remoting client and server without configuration.

# How ASP.NET can call Windows Service

This is a complete sample that demonstrates how to call a Windows Service from an ASP.NET Web API application using .NET Remoting. The "wcf" branch is a version that uses WCF instead of .NET Remoting.

Included parts are
- HTML client (JQuery)
- ASP.NET Web API 2 web server
- Windows Service
- Console client for teting purposes

See [ikriv.com](http://www.ikriv.com/dev/dotnet/AspNetCallsWinService/) for detailed description of background, implementation details and project structure.

NB: Run Visual Studio as Administrator to load ASP.NET projects properly.