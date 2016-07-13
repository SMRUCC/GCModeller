---
title: IDynamicLibraryLoader
---

# IDynamicLibraryLoader
_namespace: [RDotNET.NativeLibrary](N-RDotNET.NativeLibrary.html)_

An interface definition to hide the platform specific aspects of library loading

> There are probably projects 'out there' doing this already, but nothing 
>  is obvious from a quick search. Re-consider again if you need more features.


### Methods

#### FreeLibrary
```csharp
RDotNET.NativeLibrary.IDynamicLibraryLoader.FreeLibrary(System.IntPtr)
```
Unloads a library

|Parameter Name|Remarks|
|--------------|-------|
|handle|The pointer to the entry point of the library|


#### GetFunctionAddress
```csharp
RDotNET.NativeLibrary.IDynamicLibraryLoader.GetFunctionAddress(System.IntPtr,System.String)
```
Gets a pointer to the address of a native function in the specified loaded library

|Parameter Name|Remarks|
|--------------|-------|
|hModule|Handle of the module(library)|
|lpProcName|The name of the function sought|

_returns: Handle to the native function_

#### GetLastError
```csharp
RDotNET.NativeLibrary.IDynamicLibraryLoader.GetLastError
```
Gets the last error message from the native API used to load the library.

#### LoadLibrary
```csharp
RDotNET.NativeLibrary.IDynamicLibraryLoader.LoadLibrary(System.String)
```
Load a native library (DLL on windows, shared libraries on Linux and MacOS)

|Parameter Name|Remarks|
|--------------|-------|
|filename|The file name (short file name) of the library to load, e.g. R.dll on Windows|



