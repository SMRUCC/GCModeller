---
title: JsonSerializer
---

# JsonSerializer
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Serializes and deserializes objects into and from the JSON format.
 The @"T:Newtonsoft.Json.JsonSerializer" enables you to control how objects are encoded into JSON.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonSerializer.#ctor
```
Initializes a new instance of the @"T:Newtonsoft.Json.JsonSerializer" class.

#### Create
```csharp
Newtonsoft.Json.JsonSerializer.Create(Newtonsoft.Json.JsonSerializerSettings)
```
Creates a new @"T:Newtonsoft.Json.JsonSerializer" instance using the specified @"T:Newtonsoft.Json.JsonSerializerSettings".
 The @"T:Newtonsoft.Json.JsonSerializer" will not use default settings 
 from @"P:Newtonsoft.Json.JsonConvert.DefaultSettings".

|Parameter Name|Remarks|
|--------------|-------|
|settings|The settings to be applied to the @"T:Newtonsoft.Json.JsonSerializer".|

_returns: 
            A new @"T:Newtonsoft.Json.JsonSerializer" instance using the specified @"T:Newtonsoft.Json.JsonSerializerSettings".
            The @"T:Newtonsoft.Json.JsonSerializer" will not use default settings 
            from @"P:Newtonsoft.Json.JsonConvert.DefaultSettings".
            _

#### CreateDefault
```csharp
Newtonsoft.Json.JsonSerializer.CreateDefault(Newtonsoft.Json.JsonSerializerSettings)
```
Creates a new @"T:Newtonsoft.Json.JsonSerializer" instance using the specified @"T:Newtonsoft.Json.JsonSerializerSettings".
 The @"T:Newtonsoft.Json.JsonSerializer" will use default settings 
 from @"P:Newtonsoft.Json.JsonConvert.DefaultSettings" as well as the specified @"T:Newtonsoft.Json.JsonSerializerSettings".

|Parameter Name|Remarks|
|--------------|-------|
|settings|The settings to be applied to the @"T:Newtonsoft.Json.JsonSerializer".|

_returns: 
            A new @"T:Newtonsoft.Json.JsonSerializer" instance using the specified @"T:Newtonsoft.Json.JsonSerializerSettings".
            The @"T:Newtonsoft.Json.JsonSerializer" will use default settings 
            from @"P:Newtonsoft.Json.JsonConvert.DefaultSettings" as well as the specified @"T:Newtonsoft.Json.JsonSerializerSettings".
            _

#### Deserialize
```csharp
Newtonsoft.Json.JsonSerializer.Deserialize(Newtonsoft.Json.JsonReader,System.Type)
```
Deserializes the JSON structure contained by the specified @"T:Newtonsoft.Json.JsonReader"
 into an instance of the specified type.

|Parameter Name|Remarks|
|--------------|-------|
|reader|The @"T:Newtonsoft.Json.JsonReader" containing the object.|
|objectType|The @"T:System.Type" of object being deserialized.|

_returns: The instance of **objectType** being deserialized._

#### Deserialize``1
```csharp
Newtonsoft.Json.JsonSerializer.Deserialize``1(Newtonsoft.Json.JsonReader)
```
Deserializes the JSON structure contained by the specified @"T:Newtonsoft.Json.JsonReader"
 into an instance of the specified type.

|Parameter Name|Remarks|
|--------------|-------|
|reader|The @"T:Newtonsoft.Json.JsonReader" containing the object.|

_returns: The instance of **T** being deserialized._

#### Populate
```csharp
Newtonsoft.Json.JsonSerializer.Populate(Newtonsoft.Json.JsonReader,System.Object)
```
Populates the JSON values onto the target object.

|Parameter Name|Remarks|
|--------------|-------|
|reader|The @"T:Newtonsoft.Json.JsonReader" that contains the JSON structure to reader values from.|
|target|The target object to populate values onto.|


#### Serialize
```csharp
Newtonsoft.Json.JsonSerializer.Serialize(Newtonsoft.Json.JsonWriter,System.Object)
```
Serializes the specified @"T:System.Object" and writes the JSON structure
 to a Stream using the specified @"T:Newtonsoft.Json.JsonWriter".

|Parameter Name|Remarks|
|--------------|-------|
|jsonWriter|The @"T:Newtonsoft.Json.JsonWriter" used to write the JSON structure.|
|value|The @"T:System.Object" to serialize.|



### Properties

#### Binder
Gets or sets the @"T:System.Runtime.Serialization.SerializationBinder" used by the serializer when resolving type names.
#### CheckAdditionalContent
Gets a value indicating whether there will be a check for additional JSON content after deserializing an object.
#### ConstructorHandling
Gets or sets how constructors are used during deserialization.
#### Context
Gets or sets the @"T:System.Runtime.Serialization.StreamingContext" used by the serializer when invoking serialization callback methods.
#### ContractResolver
Gets or sets the contract resolver used by the serializer when
 serializing .NET objects to JSON and vice versa.
#### Converters
Gets a collection @"T:Newtonsoft.Json.JsonConverter" that will be used during serialization.
#### Culture
Gets or sets the culture used when reading JSON. Defaults to @"P:System.Globalization.CultureInfo.InvariantCulture".
#### DateFormatHandling
Get or set how dates are written to JSON text.
#### DateFormatString
Get or set how @"T:System.DateTime" and @"T:System.DateTimeOffset" values are formatted when writing JSON text, and the expected date format when reading JSON text.
#### DateParseHandling
Get or set how date formatted strings, e.g. "\/Date(1198908717056)\/" and "2012-03-21T05:40Z", are parsed when reading JSON.
#### DateTimeZoneHandling
Get or set how @"T:System.DateTime" time zones are handling during serialization and deserialization.
#### DefaultValueHandling
Get or set how null default are handled during serialization and deserialization.
#### EqualityComparer
Gets or sets the equality comparer used by the serializer when comparing references.
#### FloatFormatHandling
Get or set how special floating point numbers, e.g. @"F:System.Double.NaN",
 @"F:System.Double.PositiveInfinity" and @"F:System.Double.NegativeInfinity",
 are written as JSON text.
#### FloatParseHandling
Get or set how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
#### Formatting
Indicates how JSON text output is formatted.
#### MaxDepth
Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a @"T:Newtonsoft.Json.JsonReaderException".
#### MetadataPropertyHandling
Gets or sets how metadata properties are used during deserialization.
#### MissingMemberHandling
Get or set how missing members (e.g. JSON contains a property that isn't a member on the object) are handled during deserialization.
#### NullValueHandling
Get or set how null values are handled during serialization and deserialization.
#### ObjectCreationHandling
Gets or sets how objects are created during deserialization.
#### PreserveReferencesHandling
Gets or sets how object references are preserved by the serializer.
#### ReferenceLoopHandling
Get or set how reference loops (e.g. a class referencing itself) is handled.
#### ReferenceResolver
Gets or sets the @"T:Newtonsoft.Json.Serialization.IReferenceResolver" used by the serializer when resolving references.
#### StringEscapeHandling
Get or set how strings are escaped when writing JSON text.
#### TraceWriter
Gets or sets the @"T:Newtonsoft.Json.Serialization.ITraceWriter" used by the serializer when writing trace messages.
#### TypeNameAssemblyFormat
Gets or sets how a type name assembly is written and resolved by the serializer.
#### TypeNameHandling
Gets or sets how type name writing and reading is handled by the serializer.
