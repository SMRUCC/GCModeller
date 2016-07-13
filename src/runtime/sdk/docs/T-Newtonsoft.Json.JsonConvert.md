---
title: JsonConvert
---

# JsonConvert
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Provides methods for converting between common language runtime types and JSON types.



### Methods

#### DeserializeAnonymousType``1
```csharp
Newtonsoft.Json.JsonConvert.DeserializeAnonymousType``1(System.String,``0,Newtonsoft.Json.JsonSerializerSettings)
```
Deserializes the JSON to the given anonymous type using @"T:Newtonsoft.Json.JsonSerializerSettings".

|Parameter Name|Remarks|
|--------------|-------|
|value|The JSON to deserialize.|
|anonymousTypeObject|The anonymous type object.|
|settings|
            The @"T:Newtonsoft.Json.JsonSerializerSettings" used to deserialize the object.
            If this is null, default serialization settings will be used.
            |

_returns: The deserialized anonymous type from the JSON string._

#### DeserializeObject
```csharp
Newtonsoft.Json.JsonConvert.DeserializeObject(System.String,System.Type,Newtonsoft.Json.JsonSerializerSettings)
```
Deserializes the JSON to the specified .NET type using @"T:Newtonsoft.Json.JsonSerializerSettings".

|Parameter Name|Remarks|
|--------------|-------|
|value|The JSON to deserialize.|
|type|The type of the object to deserialize to.|
|settings|
            The @"T:Newtonsoft.Json.JsonSerializerSettings" used to deserialize the object.
            If this is null, default serialization settings will be used.
            |

_returns: The deserialized object from the JSON string._

#### DeserializeObject``1
```csharp
Newtonsoft.Json.JsonConvert.DeserializeObject``1(System.String,Newtonsoft.Json.JsonSerializerSettings)
```
Deserializes the JSON to the specified .NET type using @"T:Newtonsoft.Json.JsonSerializerSettings".

|Parameter Name|Remarks|
|--------------|-------|
|value|The object to deserialize.|
|settings|
            The @"T:Newtonsoft.Json.JsonSerializerSettings" used to deserialize the object.
            If this is null, default serialization settings will be used.
            |

_returns: The deserialized object from the JSON string._

#### DeserializeObjectAsync
```csharp
Newtonsoft.Json.JsonConvert.DeserializeObjectAsync(System.String,System.Type,Newtonsoft.Json.JsonSerializerSettings)
```
Asynchronously deserializes the JSON to the specified .NET type using @"T:Newtonsoft.Json.JsonSerializerSettings".
 Deserialization will happen on a new thread.

|Parameter Name|Remarks|
|--------------|-------|
|value|The JSON to deserialize.|
|type|The type of the object to deserialize to.|
|settings|
            The @"T:Newtonsoft.Json.JsonSerializerSettings" used to deserialize the object.
            If this is null, default serialization settings will be used.
            |

_returns: 
            A task that represents the asynchronous deserialize operation. The value of the TResult parameter contains the deserialized object from the JSON string.
            _

#### DeserializeObjectAsync``1
```csharp
Newtonsoft.Json.JsonConvert.DeserializeObjectAsync``1(System.String,Newtonsoft.Json.JsonSerializerSettings)
```
Asynchronously deserializes the JSON to the specified .NET type using @"T:Newtonsoft.Json.JsonSerializerSettings".
 Deserialization will happen on a new thread.

|Parameter Name|Remarks|
|--------------|-------|
|value|The JSON to deserialize.|
|settings|
            The @"T:Newtonsoft.Json.JsonSerializerSettings" used to deserialize the object.
            If this is null, default serialization settings will be used.
            |

_returns: 
            A task that represents the asynchronous deserialize operation. The value of the TResult parameter contains the deserialized object from the JSON string.
            _

#### DeserializeXmlNode
```csharp
Newtonsoft.Json.JsonConvert.DeserializeXmlNode(System.String,System.String,System.Boolean)
```
Deserializes the XmlNode from a JSON string nested in a root elment specified by **deserializeRootElementName**
 and writes a .NET array attribute for collections.

|Parameter Name|Remarks|
|--------------|-------|
|value|The JSON string.|
|deserializeRootElementName|The name of the root element to append when deserializing.|
|writeArrayAttribute|
            A flag to indicate whether to write the Json.NET array attribute.
            This attribute helps preserve arrays when converting the written XML back to JSON.
            |

_returns: The deserialized XmlNode_

#### DeserializeXNode
```csharp
Newtonsoft.Json.JsonConvert.DeserializeXNode(System.String,System.String,System.Boolean)
```
Deserializes the @"T:System.Xml.Linq.XNode" from a JSON string nested in a root elment specified by **deserializeRootElementName**
 and writes a .NET array attribute for collections.

|Parameter Name|Remarks|
|--------------|-------|
|value|The JSON string.|
|deserializeRootElementName|The name of the root element to append when deserializing.|
|writeArrayAttribute|
            A flag to indicate whether to write the Json.NET array attribute.
            This attribute helps preserve arrays when converting the written XML back to JSON.
            |

_returns: The deserialized XNode_

#### PopulateObject
```csharp
Newtonsoft.Json.JsonConvert.PopulateObject(System.String,System.Object,Newtonsoft.Json.JsonSerializerSettings)
```
Populates the object with values from the JSON string using @"T:Newtonsoft.Json.JsonSerializerSettings".

|Parameter Name|Remarks|
|--------------|-------|
|value|The JSON to populate values from.|
|target|The target object to populate values onto.|
|settings|
            The @"T:Newtonsoft.Json.JsonSerializerSettings" used to deserialize the object.
            If this is null, default serialization settings will be used.
            |


#### PopulateObjectAsync
```csharp
Newtonsoft.Json.JsonConvert.PopulateObjectAsync(System.String,System.Object,Newtonsoft.Json.JsonSerializerSettings)
```
Asynchronously populates the object with values from the JSON string using @"T:Newtonsoft.Json.JsonSerializerSettings".

|Parameter Name|Remarks|
|--------------|-------|
|value|The JSON to populate values from.|
|target|The target object to populate values onto.|
|settings|
            The @"T:Newtonsoft.Json.JsonSerializerSettings" used to deserialize the object.
            If this is null, default serialization settings will be used.
            |

_returns: 
            A task that represents the asynchronous populate operation.
            _

#### SerializeObject
```csharp
Newtonsoft.Json.JsonConvert.SerializeObject(System.Object,System.Type,Newtonsoft.Json.Formatting,Newtonsoft.Json.JsonSerializerSettings)
```
Serializes the specified object to a JSON string using a type, formatting and @"T:Newtonsoft.Json.JsonSerializerSettings".

|Parameter Name|Remarks|
|--------------|-------|
|value|The object to serialize.|
|formatting|Indicates how the output is formatted.|
|settings|The @"T:Newtonsoft.Json.JsonSerializerSettings" used to serialize the object.
            If this is null, default serialization settings will be used.|
|type|
            The type of the value being serialized.
            This parameter is used when @"T:Newtonsoft.Json.TypeNameHandling" is Auto to write out the type name if the type of the value does not match.
            Specifing the type is optional.
            |

_returns: 
            A JSON string representation of the object.
            _

#### SerializeObjectAsync
```csharp
Newtonsoft.Json.JsonConvert.SerializeObjectAsync(System.Object,Newtonsoft.Json.Formatting,Newtonsoft.Json.JsonSerializerSettings)
```
Asynchronously serializes the specified object to a JSON string using formatting and a collection of @"T:Newtonsoft.Json.JsonConverter".
 Serialization will happen on a new thread.

|Parameter Name|Remarks|
|--------------|-------|
|value|The object to serialize.|
|formatting|Indicates how the output is formatted.|
|settings|The @"T:Newtonsoft.Json.JsonSerializerSettings" used to serialize the object.
            If this is null, default serialization settings will be used.|

_returns: 
            A task that represents the asynchronous serialize operation. The value of the TResult parameter contains a JSON string representation of the object.
            _

#### SerializeXmlNode
```csharp
Newtonsoft.Json.JsonConvert.SerializeXmlNode(System.Xml.XmlNode,Newtonsoft.Json.Formatting,System.Boolean)
```
Serializes the XML node to a JSON string using formatting and omits the root object if **omitRootObject** is true.

|Parameter Name|Remarks|
|--------------|-------|
|node|The node to serialize.|
|formatting|Indicates how the output is formatted.|
|omitRootObject|Omits writing the root object.|

_returns: A JSON string of the XmlNode._

#### SerializeXNode
```csharp
Newtonsoft.Json.JsonConvert.SerializeXNode(System.Xml.Linq.XObject,Newtonsoft.Json.Formatting,System.Boolean)
```
Serializes the @"T:System.Xml.Linq.XNode" to a JSON string using formatting and omits the root object if **omitRootObject** is true.

|Parameter Name|Remarks|
|--------------|-------|
|node|The node to serialize.|
|formatting|Indicates how the output is formatted.|
|omitRootObject|Omits writing the root object.|

_returns: A JSON string of the XNode._

#### ToString
```csharp
Newtonsoft.Json.JsonConvert.ToString(System.Object)
```
Converts the @"T:System.Object" to its JSON string representation.

|Parameter Name|Remarks|
|--------------|-------|
|value|The value to convert.|

_returns: A JSON string representation of the @"T:System.Object"._


### Properties

#### DefaultSettings
Gets or sets a function that creates default @"T:Newtonsoft.Json.JsonSerializerSettings".
 Default settings are automatically used by serialization methods on @"T:Newtonsoft.Json.JsonConvert",
 and @"M:Newtonsoft.Json.Linq.JToken.ToObject``1" and @"M:Newtonsoft.Json.Linq.JToken.FromObject(System.Object)" on @"T:Newtonsoft.Json.Linq.JToken".
 To serialize without using any default settings create a @"T:Newtonsoft.Json.JsonSerializer" with
 @"M:Newtonsoft.Json.JsonSerializer.Create".
#### False
Represents JavaScript's boolean value false as a string. This field is read-only.
#### NaN
Represents JavaScript's NaN as a string. This field is read-only.
#### NegativeInfinity
Represents JavaScript's negative infinity as a string. This field is read-only.
#### Null
Represents JavaScript's null as a string. This field is read-only.
#### PositiveInfinity
Represents JavaScript's positive infinity as a string. This field is read-only.
#### True
Represents JavaScript's boolean value true as a string. This field is read-only.
#### Undefined
Represents JavaScript's undefined as a string. This field is read-only.
