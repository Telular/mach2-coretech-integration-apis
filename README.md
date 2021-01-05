# Mach2 CoreTech Integration API's

## Applications

These are the API projects that are currently stored in this repo.


### Asset Link Global API

API to communicate with the Asset Link Global system that will host the data links to satellite communication devices that will be utilized by a new class of Tank devices.
The API is NOT a REST API and has a very specific and very opinionated data return layer.  That said the provided API provides the means to issue commands against the API "File" types of devices, groups (grp), moments, and remote.

### TankLink Notifier API Net

This is a .Net framework based web application that provides the REST interface for Tank Utility's corporation to POST tank readings and tank configuration changes to one of two REST API methods.

### Tank Utility API
API to communicate with the Tank Utility Corporation tank monitoring system.  The REST API allows for setting tank configuration data and retrieving tank configuration and readings data.

### Tank Utility Interface
Telular in-house interface to the TankLink data system.  Provides a way of moving messages from the TankUtility Topic's tank readings and tank configuration changes subscriptions into TankLink.


