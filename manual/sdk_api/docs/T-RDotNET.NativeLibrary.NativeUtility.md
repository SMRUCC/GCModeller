---
title: NativeUtility
---

# NativeUtility
_namespace: [RDotNET.NativeLibrary](N-RDotNET.NativeLibrary.html)_

Collection of utility methods for operating systems.



### Methods

#### ExecCommand
```csharp
RDotNET.NativeLibrary.NativeUtility.ExecCommand(System.String,System.String)
```
Execute a command in a new process

|Parameter Name|Remarks|
|--------------|-------|
|processName|Process name e.g. "uname"|
|arguments|Arguments e.g. "-s"|

_returns: The output of the command to the standard output stream_

#### FindRHome
```csharp
RDotNET.NativeLibrary.NativeUtility.FindRHome(System.String)
```
Try to locate the directory path to use for the R_HOME environment variable. This is used by R.NET by default; users may want to use it to diagnose problematic behaviors.

|Parameter Name|Remarks|
|--------------|-------|
|rPath|Optional path to the directory containing the R shared library. This is ignored unless on a Unix platform (i.e. ignored on Windows and MacOS)|

_returns: The path that R.NET found suitable as a candidate for the R_HOME environment_

#### FindRPath
```csharp
RDotNET.NativeLibrary.NativeUtility.FindRPath
```
Attempt to find a suitable path to the R shared library. This is used by R.NET by default; users may want to use it to diagnose problematic behaviors.
_returns: The path to the directory where the R shared library is expected to be_

#### FindRPathFromRegistry
```csharp
RDotNET.NativeLibrary.NativeUtility.FindRPathFromRegistry
```
Windows-only function; finds in the Windows registry the path to the most recently installed R binaries.
_returns: The path, such as_

#### GetPlatform
```csharp
RDotNET.NativeLibrary.NativeUtility.GetPlatform
```
Gets the platform on which the current process runs.
_returns: The current platform._
> 
>  @"P:System.Environment.OSVersion"'s platform is not @"F:System.PlatformID.MacOSX" even on Mac OS X.
>  This method returns @"F:System.PlatformID.MacOSX" when the current process runs on Mac OS X.
>  This method uses UNIX's uname command to check the operating system,
>  so this method cannot check the OS correctly if the PATH environment variable is changed (will returns @"F:System.PlatformID.Unix").
>  

#### GetRDllFileName
```csharp
RDotNET.NativeLibrary.NativeUtility.GetRDllFileName
```
Gets the default file name of the R library on the supported platforms.
_returns: R dll file name_

#### GetRHomeEnvironmentVariable
```csharp
RDotNET.NativeLibrary.NativeUtility.GetRHomeEnvironmentVariable
```
Gets the value, if any, of the R_HOME environment variable of the current process
_returns: The value, or null if not set_

#### SetEnvironmentVariables
```csharp
RDotNET.NativeLibrary.NativeUtility.SetEnvironmentVariables(System.String,System.String)
```
Sets the PATH and R_HOME environment variables if needed.

|Parameter Name|Remarks|
|--------------|-------|
|rPath|The path of the directory containing the R native library. 
 If null (default), this function tries to locate the path via the Windows registry, or commonly used locations on MacOS and Linux|
|rHome|The path for R_HOME. If null (default), the function checks the R_HOME environment variable. If none is set, 
 the function uses platform specific sensible default behaviors.|

> 
>  This function has been designed to limit the tedium for users, while allowing custom settings for unusual installations.
>  


### Properties

#### IsUnix
Is the platform a unix like (Unix or MacOX)
