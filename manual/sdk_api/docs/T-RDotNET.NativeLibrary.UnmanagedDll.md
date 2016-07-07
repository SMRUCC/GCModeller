---
title: UnmanagedDll
---

# UnmanagedDll
_namespace: [RDotNET.NativeLibrary](N-RDotNET.NativeLibrary.html)_

A proxy for unmanaged dynamic link library (DLL).



### Methods

#### #ctor
```csharp
RDotNET.NativeLibrary.UnmanagedDll.#ctor(System.String)
```
Creates a proxy for the specified dll.

|Parameter Name|Remarks|
|--------------|-------|
|dllName|The DLL's name.|


#### DangerousGetHandle
```csharp
RDotNET.NativeLibrary.UnmanagedDll.DangerousGetHandle(System.String)
```
Gets the handle of the specified entry.

|Parameter Name|Remarks|
|--------------|-------|
|entryPoint|The name of function.|

_returns: The handle._

#### Dispose
```csharp
RDotNET.NativeLibrary.UnmanagedDll.Dispose(System.Boolean)
```
Frees the native library this objects represents

|Parameter Name|Remarks|
|--------------|-------|
|disposing|-|


#### GetFunction``1
```csharp
RDotNET.NativeLibrary.UnmanagedDll.GetFunction``1(System.String)
```
Creates the delegate function for the specified function defined in the DLL.

|Parameter Name|Remarks|
|--------------|-------|
|entryPoint|The name of the function exported by the DLL|

_returns: The delegate._

#### ReleaseHandle
```csharp
RDotNET.NativeLibrary.UnmanagedDll.ReleaseHandle
```
Frees the native library this objects represents
_returns: The result of the call to FreeLibrary_


### Properties

#### DllFilename
Gets the Dll file name used for this native Dll wrapper.
#### IsInvalid
Gets whether the current handle is equal to the invalid handle
