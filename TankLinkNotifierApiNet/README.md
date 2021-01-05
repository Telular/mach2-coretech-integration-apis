# TankLink Notifier API - .NET

.NET 4.7.2 web application that implements a REST interface that allows a calling application to POST Tank Utility tank readings and or tank configuration changes.  The API is reachable over the internet via the dataflow.tanklink.com URL.
The application takes web request and stores the POST-ed tank reading or configuration change to a Azure service bus mo-devicemessages-tankutility-topic.

## Prerequisites

Visual Studio 2019
Compile and run it as is.

### TankNotifierApiNet

.NET 4.7.2 web application that implements a REST interface that allows a calling application to POST Tank Utility tank readings and or tank configuration changes.  The API is reachable over the internet via the dataflow.tanklink.com URL.

### RedisCacheLib

The (.NET 4.7.2) library that encapsulates the interface to Redis Cache (Azure Cache for Redis) that is utilized by the web app to store OAuth 2.0 API tokens and Tank Utlity device Id's (short and long).

