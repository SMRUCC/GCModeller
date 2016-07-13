---
title: ITraceWriter
---

# ITraceWriter
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Represents a trace writer.



### Methods

#### Trace
```csharp
Newtonsoft.Json.Serialization.ITraceWriter.Trace(System.Diagnostics.TraceLevel,System.String,System.Exception)
```
Writes the specified trace level, message and optional exception.

|Parameter Name|Remarks|
|--------------|-------|
|level|The @"T:System.Diagnostics.TraceLevel" at which to write this trace.|
|message|The trace message.|
|ex|The trace exception. This parameter is optional.|



### Properties

#### LevelFilter
Gets the @"T:System.Diagnostics.TraceLevel" that will be used to filter the trace messages passed to the writer.
 For example a filter level of 'Info' will exclude 'Verbose' messages and include 'Info',
 'Warning' and 'Error' messages.
