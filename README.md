# gateway-controller
This C# library provides basic methods to start and stop IB Gateway from your .NET application.

```cs
// Create gateway object
Gateway gateway = new Gateway();

// Start the gateway, it will take some time
gateway.Start("963", "myusername", "mypassword", false);

// Do some work here
// ....

// Stop the gateway now. I hope you have telnet feature turned on
gateway.Stop();
```

## Install
* Use a [NuGet](https://www.nuget.org/packages/GatewayController/) package
* Or use the code in this repository

## Prequisites
* You should have telnet enabled which is required to stop the IB Gateway.

## How it works
This library is just a wrapper over [IB Controller](https://github.com/ib-controller/ib-controller/). It means it just edits and calls batch files from the IB Controller.

## Misc
* The package creates *IBController* directory in the root of your project. All files should have set **Copy to Output Directory** to **Copy always**.
* You may need to edit configuration files in the *IBController* directory to fit your environment.
* This library was tested against Gateway version 963 installed in default directory *C:\Jts\\*
