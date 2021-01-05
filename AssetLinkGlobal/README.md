# Asset Link Global API

.NET 4.7.1 DLL and an MS Unit Test project and bulk provisioning console app that illustrates how to call the API and get returned data in strongly typed .NET classes.

## Prerequisites

Visual Studio 2019
Compile and run it as is.

### AssetLinkBulkProvisioner

Console application that takes a CSV file with ESN data in it and calls the AssetLinkApi to provision the device.  This application uses the AssetLinkApi library to provision a satellite device.  Used for stand alone AssetLink devices that do not exist in Mach2 CoreTech as devices and therefore can't be provisioned thru the device management API.

### AssetLinkGlobalApiLib

The (.NET 4.7.1) library that encapsulates the AssetLink API.

### AssetLinkGlobalTests

Integration unit tests that exercise the AssetLink Global API.  All supported methods are demonstrated including provisioning.  Provisioning tests are set to "Ignore" to keep from running them over and over.
