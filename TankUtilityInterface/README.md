# TankLink - Tank Utility Interface

Console application that reads messages from the Tank Utility service bus topic subscriptions (TANK_UTILITY_READINGS), formats them into a Comm Packete Manager format, and forwards them to the TankLink system.

## Prerequisites

Visual Studio 2019
Compile and run it as is.

### TankUtilityInterface

Console application that performs the bulk of the work

### CommPkt Library

CommPkt.dll is a pre-compiled .NET library that encapsulates the components and data structures for interfacing to the TankLink data system.

