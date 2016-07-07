---
title: REngine
---

# REngine
_namespace: [RDotNET](N-RDotNET.html)_

REngine handles R environment through evaluation of R statement.



### Methods

#### #ctor
```csharp
RDotNET.REngine.#ctor(System.String,System.String)
```
Create a new REngine instance

|Parameter Name|Remarks|
|--------------|-------|
|id|The identifier of this object|
|dll|The name of the file that is the shared R library, e.g. "R.dll"|


#### BuildRArgv
```csharp
RDotNET.REngine.BuildRArgv(RDotNET.StartupParameter)
```
Creates the command line arguments corresponding to the specified startup parameters

|Parameter Name|Remarks|
|--------------|-------|
|parameter|-|

> While not obvious from the R documentation, it seems that command line arguments need to be passed 
>  to get the startup parameters taken into account. Passing the StartupParameter to the API seems not to work as expected. 
>  While this function may appear like an oddity to a reader, it proved necessary to the initialisation of the R engine 
>  after much trial and error.

#### ClearGlobalEnvironment
```csharp
RDotNET.REngine.ClearGlobalEnvironment(System.Boolean,System.Boolean,System.Boolean,System.Boolean,System.String[])
```
Removes variables from the R global environment, and whether garbage collections should be forced

|Parameter Name|Remarks|
|--------------|-------|
|garbageCollectR|if true (default) request an R garbage collection. This happens after the .NET garbage collection if both requested|
|garbageCollectDotNet|If true (default), triggers CLR garbage collection and wait for pending finalizers.|
|removeHiddenRVars|Should hidden variables (starting with '.', such as '.Random.seed') be removed. Default is false.|
|detachPackages|If true, detach some packages and other attached resources. Default is false. See 'detach' function in R|
|toDetach|names of resources to dettach, e.g. an array of names such as 'mpg', 'package:lattice'. 
 If null, entries found in 'search()' between the first item and 'package:base' are detached. See 'search' function documentation in R|


#### CreateInstance
```csharp
RDotNET.REngine.CreateInstance(System.String,System.String)
```
Creates a new instance that handles R.DLL.

|Parameter Name|Remarks|
|--------------|-------|
|id|ID.|
|dll|The file name of the library to load, e.g. "R.dll" for Windows. You should usually not provide this optional parameter|

_returns: The engine._

#### Defer
```csharp
RDotNET.REngine.Defer(System.IO.Stream)
```
Evaluates a statement in the given stream.

|Parameter Name|Remarks|
|--------------|-------|
|stream|The stream.|

_returns: Each evaluation._

#### Dispose
```csharp
RDotNET.REngine.Dispose(System.Boolean)
```
Dispose of this REngine, including using the native R API to clean up, if the parameter is true

|Parameter Name|Remarks|
|--------------|-------|
|disposing|if true, release native resources, using the native R API to clean up.|


#### Evaluate
```csharp
RDotNET.REngine.Evaluate(System.IO.Stream)
```
Evaluates a statement in the given stream.

|Parameter Name|Remarks|
|--------------|-------|
|stream|The stream.|

_returns: Last evaluation._

#### ForceGarbageCollection
```csharp
RDotNET.REngine.ForceGarbageCollection
```
Forces garbage collection.

#### GetDangerousChar
```csharp
RDotNET.REngine.GetDangerousChar(System.String)
```
Gets the value of a character string

|Parameter Name|Remarks|
|--------------|-------|
|varname|The variable name exported by the R dynamic library, e.g. R_ParseErrorMsg|

_returns: The Unicode equivalent of the native ANSI string_

#### GetDangerousInt32
```csharp
RDotNET.REngine.GetDangerousInt32(System.String)
```
Gets the value of a a global variable in native memory, of type int or compatible (e.g. uintptr_t)

|Parameter Name|Remarks|
|--------------|-------|
|varname|variable name|

_returns: The value, as read by Marshal.ReadInt32_

#### GetInstance
```csharp
RDotNET.REngine.GetInstance(System.String,System.Boolean,RDotNET.StartupParameter,RDotNET.Devices.ICharacterDevice)
```
Gets a reference to the R engine, creating and initializing it if necessary. In most cases users need not provide any parameter to this method.

|Parameter Name|Remarks|
|--------------|-------|
|dll|The file name of the library to load, e.g. "R.dll" for Windows. You usually do not need need to provide this optional parameter|
|initialize|Initialize the R engine after its creation. Default is true|
|parameter|If 'initialize' is 'true', you can optionally specify the specific startup parameters for the R native engine|
|device|If 'initialize' is 'true', you can optionally specify a character device for the R engine to use|

_returns: The engine._

#### GetPredefinedSymbol
```csharp
RDotNET.REngine.GetPredefinedSymbol(System.String)
```
Gets the predefined symbol with the specified name.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name.|

_returns: The symbol._

#### GetSymbol
```csharp
RDotNET.REngine.GetSymbol(System.String,RDotNET.REnvironment)
```
Gets a symbol defined in the global environment.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name.|
|environment|The environment. If null is passed, @"P:RDotNET.REngine.GlobalEnvironment" is used.|

_returns: The symbol._

#### Initialize
```csharp
RDotNET.REngine.Initialize(RDotNET.StartupParameter,RDotNET.Devices.ICharacterDevice,System.Boolean)
```
Initialize this REngine object. Only the first call has an effect. Subsequent calls to this function are ignored.

|Parameter Name|Remarks|
|--------------|-------|
|parameter|The optional startup parameters|
|device|The optional character device to use for the R engine|
|setupMainLoop|if true, call the functions to initialise the embedded R|


#### OnDisposing
```csharp
RDotNET.REngine.OnDisposing(System.EventArgs)
```
Called on disposing of this REngine

|Parameter Name|Remarks|
|--------------|-------|
|e|-|


#### ProcessRDllFileName
```csharp
RDotNET.REngine.ProcessRDllFileName(System.String)
```
if the parameter is null or empty string, return the default names of the R shared library file depending on the platform

|Parameter Name|Remarks|
|--------------|-------|
|dll|The name of the library provided, possibly null or empty|

_returns: A candidate for the file name of the R shared library_

#### SetCommandLineArguments
```csharp
RDotNET.REngine.SetCommandLineArguments(System.String[])
```
Sets the command line arguments.

|Parameter Name|Remarks|
|--------------|-------|
|args|The arguments.|


#### SetDangerousInt32
```csharp
RDotNET.REngine.SetDangerousInt32(System.String,System.Int32)
```
Set a global variable in native memory, of type int or compatible (e.g. uintptr_t)

|Parameter Name|Remarks|
|--------------|-------|
|varname|variable name|
|value|Value.|


#### SetEnvironmentVariables
```csharp
RDotNET.REngine.SetEnvironmentVariables(System.String,System.String)
```
Perform the necessary setup for the PATH and R_HOME environment variables.

|Parameter Name|Remarks|
|--------------|-------|
|rPath|The path of the directory containing the R native library. 
 If null (default), this function tries to locate the path via the Windows registry, or commonly used locations on MacOS and Linux|
|rHome|The path for R_HOME. If null (default), the function checks the R_HOME environment variable. If none is set, 
 the function uses platform specific sensible default behaviors.|

> 
>  This function has been designed to limit the tedium for users, while allowing custom settings for unusual installations.
>  

#### SetSymbol
```csharp
RDotNET.REngine.SetSymbol(System.String,RDotNET.SymbolicExpression,RDotNET.REnvironment)
```
Assign a value to a name in a specific environment.

|Parameter Name|Remarks|
|--------------|-------|
|name|The name.|
|expression|The symbol.|
|environment|The environment. If null is passed, @"P:RDotNET.REngine.GlobalEnvironment" is used.|



### Properties

#### BaseNamespace
Gets the base environment.
#### Disposed
Gets whether this object has been disposed of already.
#### DllVersion
Gets the version of R.DLL.
#### EmptyEnvironment
Gets the root environment.
#### EnableLock
Gets/sets whether the call to Preserve and Unpreserve on symbolic expressions 
 should be using a lock to prevent thread concurrency issues. Default is false;
#### EngineName
Gets the name of the R engine instance (singleton).
#### geterrmessage
A cache of the unevaluated R expression 'geterrmessage'
#### GlobalEnvironment
Gets the global environment.
#### ID
Gets the ID of this instance.
#### IsRunning
Gets whether this instance is running.
#### LastErrorMessage
Gets the last error message in the R engine; see R function geterrmessage.
#### NaString
SEXP representing NA for strings (character vectors in R terminology).
#### NaStringPointer
Native pointer to the SEXP representing NA for strings (character vectors in R terminology).
#### NilValue
Gets the NULL value.
#### UnboundValue
Gets the unbound value.
