---
title: IJsonLineInfo
---

# IJsonLineInfo
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Provides an interface to enable a class to return line and position information.



### Methods

#### HasLineInfo
```csharp
Newtonsoft.Json.IJsonLineInfo.HasLineInfo
```
Gets a value indicating whether the class can return line information.
_returns: true if LineNumber and LinePosition can be provided; otherwise, false.
            _


### Properties

#### LineNumber
Gets the current line number.
#### LinePosition
Gets the current line position.
