# Asset Link Global API

.NET 4.7.1 DLL and an MS Unit Test project and bulk provisioning console app that illustrates how to call the API and get returned data in strongly typed .NET classes.

## Prerequisites

Visual Studio 2019
Compile and run it as is.

### AssetLinkBulkProvisioner

Console application that takes a CSV file with ESN data in it and calls the AssetLinkApi to provision the device.  This application uses the AssetLinkApi library and does call the Mach2 CoreTech library since the devices may not exist yet.

### AssetLinkGlobalApiLib

The (.NET 4.7.1) library that encapsulates the AssetLink API.

### AssetLinkGlobalTests

Integration unit tests that exercise the AssetLink Global API.  All support methods are demonstrated including provisioning.  Provisioning tests are set to "Ignore" to keep from running them over and over.
