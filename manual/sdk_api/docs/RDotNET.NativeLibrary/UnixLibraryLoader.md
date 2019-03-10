﻿# UnixLibraryLoader
_namespace: [RDotNET.NativeLibrary](./index.md)_





### Methods

#### GetLastError
```csharp
RDotNET.NativeLibrary.UnixLibraryLoader.GetLastError
```
Gets the last error. NOTE: according to http://tldp.org/HOWTO/Program-Library-HOWTO/dl-libraries.html, returns NULL if called more than once after dlopen.

_returns: The last error._


