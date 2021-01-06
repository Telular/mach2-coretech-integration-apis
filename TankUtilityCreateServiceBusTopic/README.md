# Tank Utility - Create Service Bus Topic

Console application that connects to service bus and will create or re-create the service bus topic and subscriptions used in the TankUtility dataflow.
The topic, mo-devicemessages-tankutility-topic, and two related subscriptions, TANK-UTILITY-READINGS and TANK-UTILITY-CONFIGS, are integral to the operation of the TankLinkNotifier and TankUtilityInterface apps.

## Prerequisites

Visual Studio 2019
Compile and run it as is.

### TankUtilityCreateServiceBusTopic

Console application that performs the utilizes the same logic to create or recreate the topic and its associated subscriptions as used in the TankLinkNotifier web app.

