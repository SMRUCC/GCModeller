---
title: ConvertUtils
---

# ConvertUtils
_namespace: [Newtonsoft.Json.Utilities](N-Newtonsoft.Json.Utilities.html)_





### Methods

#### ConvertOrCast
```csharp
Newtonsoft.Json.Utilities.ConvertUtils.ConvertOrCast(System.Object,System.Globalization.CultureInfo,System.Type)
```
Converts the value to the specified type. If the value is unable to be converted, the
 value is checked whether it assignable to the specified type.

|Parameter Name|Remarks|
|--------------|-------|
|initialValue|The value to convert.|
|culture|The culture to use when converting.|
|targetType|The type to convert or cast the value to.|

_returns: 
            The converted type. If conversion was unsuccessful, the initial value
            is returned if assignable to the target type.
            _


