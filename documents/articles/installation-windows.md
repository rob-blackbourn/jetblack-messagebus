# Installation - Windows

## Download and Unpack

First download and install the message bus service.

To get the distributor go to the
[Releases](https://github.com/rob-blackbourn/jetblack-messagebus/releases) 
page of the repo and download the distributor for your platform: typically.
distributor-5.0.0-win10-x64.zip. Unzip the archive into a folder of your choice.

The examples in the documentation assume the distributer has been
unpacked to `C:\distributor` to keep the paths simple.

```bat
C:\distributor>dir
 Volume in drive C has no label.
 Volume Serial Number is 3A0A-FCE5

 Directory of C:\distributor

31/07/2021  09:39    <DIR>          .
31/07/2021  09:39    <DIR>          ..
31/07/2021  08:58        70,671,627 JetBlack.MessageBus.Distributor.exe
31/07/2021  08:58            24,404 JetBlack.MessageBus.Distributor.pdb
               2 File(s)     70,700,256 bytes
               2 Dir(s)  67,376,578,560 bytes free
```

To start the distributor double-click on
`JetBlack.MessageBus.Distributor.exe` to start it. The first time
the service is run Windows will pop up a security alert indicating
the service wants to access a network port. Click on "Allow access"
to continue.

## Add a Configuration File

Optionally create a file called `appsettings.json` in the installation folder
using the default configuration given [here](configuration.md).

Now create the file `distributor.bat` with the following contents.

```bat
C:\distributor\JetBlack.MessageBus.Distributor.exe C:\distributor\appsettings.json
```
