# MetaWear NetStandard
This plugin implements the [IBluetoothLeGatt](https://github.com/mbientlab/MetaWear-SDK-CSharp/blob/1.0.0/MetaWear/Impl/Platform/IBluetoothLeGatt.cs) 
and [ILibraryIO](https://github.com/mbientlab/MetaWear-SDK-CSharp/blob/1.0.0/MetaWear/Impl/Platform/ILibraryIO.cs) interfaces for 
.NET Standard2.0 compatible applications.

Developers buidling Windows 10 specific applications should use the [Windows 10 plugin](https://github.com/mbientlab/MetaWear-SDK-CSharp-Plugin-Win10) instead.

# Install
Use the Package Manager console to install the [MetaWear.CSharp.NetStandard](https://www.nuget.org/packages/MetaWear.CSharp.NetStandard/) 
package in addition to the ``MetaWear.CSharp`` package:

```bat
PM> Install-Package MetaWear.CSharp
PM> Install-Package MetaWear.CSharp.NetStandard
```

Or, manually add the entries to the *.csproj file:

```xml
  <ItemGroup>
    <PackageReference Include="MetaWear.CSharp" Version="[1.0.15, 2.0)" />
    <PackageReference Include="MetaWear.CSharp.NetStandard" Version="[1.0.0, 2.0)" />
  </ItemGroup>
```
Linux users will also need to compile the [Warble](https://github.com/mbientlab/Warble) library on their target machine and ensure that the 
shared library is discoverable by the ``dotnet`` application.

# Usage
Call ``Application.GetMetaWearBoard`` with the MAC address of the device you are communicating with.  You can use the SDK features, as 
outlined in the SDK [developers' guide](https://mbientlab.com/csdocs/1/metawearboard.html), with the returned ``IMetaWearBoard`` object.  

```csharp
using MbientLab.MetaWear.NetStandard;
using System;
using System.Threading.Tasks;

namespace MetaWear.NETCore {
    class Program {
        static void Main(string[] args) {
            MainAsync(args).Wait();
        }

        private static async Task MainAsync(string[] args) {
            try {
                var metawear = Application.GetMetaWearBoard(args[1]);
                await metawear.InitializeAsync();
            } catch(Exception e) {
                Console.WriteLine("error: " + e.Message);
            }
        }
    }
}
```
