# Official Africa's Talking VB.NET API wrapper  

The Africa's Talking VB.NET API wrapper provides convenient access to the Africa's Talking API from applications written in VB.NET. With support for .~~NET45, .NET46 and~~ .NET Standard 2.0. 

## Documentation 
Take a look at the [API docs here](http://docs.africastalking.com/) for more information. 

## Installation Options
1. #### Using Visual Studio IDE :TBD


2. #### Using .NET CLI 

+ From the _command prompt/powershell window_ opened in your project directory, key in the following and press *Enter*. 
```powershell 
 dotnet add package AfricasTalking.NET.VB --version 2.0.0
```
> Ensure you have the latest version of the package. Visit [Nuget](https://www.nuget.org/packages/AfricasTalking.NET.VB/) for more info on the latest release of this package. 

3. #### Using Nuget Package Manger Console 

+ On your Nuget package manager console,key in the following and press *Enter* 
```powershell 
Install-Package AfricasTalking.NET.VB -Version 2.0.0
```
> Ensure you have the latest version of the package. Visit [Nuget](https://www.nuget.org/packages/AfricasTalking.NET.VB/) for more info on the latest release of this package

## Usage 

+ To use this package ensure you add the following `using` statement to your project file: 
```vb
Imports AfricasTalkingGateway
```

The package needs to be configured with your Africa's Talking username and API key (which you can get from the dashboard). 

```vb
Dim gateway As New AfricasTalkingGateway(username, apikey)
'For Sandbox use sandbox as username and your sandbox APIKEY

```
TODO: The rest of the docs