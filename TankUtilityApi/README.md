# Tank Utility API

.NET 4.7.1 DLL and an MS Unit Test project that illustrates how to call the TankUtility REST API and process returned JSON data.

## Prerequisites

Visual Studio 2019
Compile and run it as is.

###  SkyBitzApiLib

.NET 4.7.1 DLL wrapper that encapsulates sending POST request to the TankLinkNotifier (a.k.a. SkyBitzAPI).  This is used by the integration unit tests to exercise the REST api.

###  SkyBitzApiTests

Integration unit tests that exercise the TankLinkNotifier (a.k.a. SkyBitzAPI) API.  The tests post both tank readings and configuration changes.  Change the app config setting for which server to hit (localhost or PROD).
The unit tests are also designed to run loadtesting against the API.  These tests provide for a convenient way to create messages on the corresponding Azure service bus topic.

###  TankUtilityApiLib

.NET 4.7.1 DLL that encapsulates the Tank Utility REST API

###  TankUtilityApiTests

Integration unit tests that exercise the Tank Utility REST API.

###  TankUtilityTopicReader

Console application that demonstrates fetching tank readings and tank configuration messages from their corresponding Azure service bus topic subscriptions.

