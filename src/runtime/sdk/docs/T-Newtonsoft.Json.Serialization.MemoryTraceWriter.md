---
title: MemoryTraceWriter
---

# MemoryTraceWriter
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Represents a trace writer that writes to memory. When the trace message limit is
 reached then old trace messages will be removed as new messages are added.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Serialization.MemoryTraceWriter.#ctor
```
Initializes a new instance of the @"T:Newtonsoft.Json.Serialization.MemoryTraceWriter" class.

#### GetTraceMessages
```csharp
Newtonsoft.Json.Serialization.MemoryTraceWriter.GetTraceMessages
```
Returns an enumeration of the most recent trace messages.
_returns: An enumeration of the most recent trace messages._

#### ToString
```csharp
Newtonsoft.Json.Serialization.MemoryTraceWriter.ToString
```
Returns a @"T:System.String" of the most recent trace messages.
_returns: 
            A @"T:System.String" of the most recent trace messages.
            _

#### Trace
```csharp
Newtonsoft.Json.Serialization.MemoryTraceWriter.Trace(System.Diagnostics.TraceLevel,System.String,System.Exception)
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
