---
title: JsonSerializerSettings
---

# JsonSerializerSettings
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Specifies the settings on a @"T:Newtonsoft.Json.JsonSerializer" object.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonSerializerSettings.#ctor
```
Initializes a new instance of the @"T:Newtonsoft.Json.JsonSerializerSettings" class.


### Properties

#### Binder
Gets or sets the @"T:System.Runtime.Serialization.SerializationBinder" used by the serializer when resolving type names.
#### CheckAdditionalContent
Gets a value indicating whether there will be a check for additional content after deserializing an object.
#### ConstructorHandling
Gets or sets how constructors are used during deserialization.
#### Context
Gets or sets the @"T:System.Runtime.Serialization.StreamingContext" used by the serializer when invoking serialization callback methods.
#### ContractResolver
Gets or sets the contract resolver used by the serializer when
 serializing .NET objects to JSON and vice versa.
#### Converters
Gets or sets a @"T:Newtonsoft.Json.JsonConverter" collection that will be used during serialization.
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
Gets or sets how null default are handled during serialization and deserialization.
#### EqualityComparer
Gets or sets the equality comparer used by the serializer when comparing references.
#### Error
Gets or sets the error handler called during serialization and deserialization.
#### FloatFormatHandling
Get or set how special floating point numbers, e.g. @"F:System.Double.NaN",
 @"F:System.Double.PositiveInfinity" and @"F:System.Double.NegativeInfinity",
 are written as JSON.
#### FloatParseHandling
Get or set how floating point numbers, e.g. 1.0 and 9.9, are parsed when reading JSON text.
#### Formatting
Indicates how JSON text output is formatted.
#### MaxDepth
Gets or sets the maximum depth allowed when reading JSON. Reading past this depth will throw a @"T:Newtonsoft.Json.JsonReaderException".
#### MetadataPropertyHandling
Gets or sets how metadata properties are used during deserialization.
#### MissingMemberHandling
Gets or sets how missing members (e.g. JSON contains a property that isn't a member on the object) are handled during deserialization.
#### NullValueHandling
Gets or sets how null values are handled during serialization and deserialization.
#### ObjectCreationHandling
Gets or sets how objects are created during deserialization.
#### PreserveReferencesHandling
Gets or sets how object references are preserved by the serializer.
#### ReferenceLoopHandling
Gets or sets how reference loops (e.g. a class referencing itself) is handled.
#### ReferenceResolver
Gets or sets the @"T:Newtonsoft.Json.Serialization.IReferenceResolver" used by the serializer when resolving references.
#### ReferenceResolverProvider
Gets or sets a function that creates the @"T:Newtonsoft.Json.Serialization.IReferenceResolver" used by the serializer when resolving references.
#### StringEscapeHandling
Get or set how strings are escaped when writing JSON text.
#### TraceWriter
Gets or sets the @"T:Newtonsoft.Json.Serialization.ITraceWriter" used by the serializer when writing trace messages.
#### TypeNameAssemblyFormat
Gets or sets how a type name assembly is written and resolved by the serializer.
#### TypeNameHandling
Gets or sets how type name writing and reading is handled by the serializer.
