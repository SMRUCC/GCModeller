---
title: DefaultContractResolver
---

# DefaultContractResolver
_namespace: [Newtonsoft.Json.Serialization](N-Newtonsoft.Json.Serialization.html)_

Used by @"T:Newtonsoft.Json.JsonSerializer" to resolves a @"T:Newtonsoft.Json.Serialization.JsonContract" for a given @"T:System.Type".



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.#ctor(System.Boolean)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Serialization.DefaultContractResolver" class.

|Parameter Name|Remarks|
|--------------|-------|
|shareCache|
            If set to true the @"T:Newtonsoft.Json.Serialization.DefaultContractResolver" will use a cached shared with other resolvers of the same type.
            Sharing the cache will significantly improve performance with multiple resolver instances because expensive reflection will only
            happen once. This setting can cause unexpected behavior if different instances of the resolver are suppose to produce different
            results. When set to false it is highly recommended to reuse @"T:Newtonsoft.Json.Serialization.DefaultContractResolver" instances with the @"T:Newtonsoft.Json.JsonSerializer".
            |


#### CreateArrayContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateArrayContract(System.Type)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonArrayContract" for the given type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: A @"T:Newtonsoft.Json.Serialization.JsonArrayContract" for the given type._

#### CreateConstructorParameters
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateConstructorParameters(System.Reflection.ConstructorInfo,Newtonsoft.Json.Serialization.JsonPropertyCollection)
```
Creates the constructor parameters.

|Parameter Name|Remarks|
|--------------|-------|
|constructor|The constructor to create properties for.|
|memberProperties|The type's member properties.|

_returns: Properties for the given @"T:System.Reflection.ConstructorInfo"._

#### CreateContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateContract(System.Type)
```
Determines which contract type is created for the given type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: A @"T:Newtonsoft.Json.Serialization.JsonContract" for the given type._

#### CreateDictionaryContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateDictionaryContract(System.Type)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonDictionaryContract" for the given type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: A @"T:Newtonsoft.Json.Serialization.JsonDictionaryContract" for the given type._

#### CreateDynamicContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateDynamicContract(System.Type)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonDynamicContract" for the given type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: A @"T:Newtonsoft.Json.Serialization.JsonDynamicContract" for the given type._

#### CreateISerializableContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateISerializableContract(System.Type)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonISerializableContract" for the given type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: A @"T:Newtonsoft.Json.Serialization.JsonISerializableContract" for the given type._

#### CreateLinqContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateLinqContract(System.Type)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonLinqContract" for the given type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: A @"T:Newtonsoft.Json.Serialization.JsonLinqContract" for the given type._

#### CreateMemberValueProvider
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateMemberValueProvider(System.Reflection.MemberInfo)
```
Creates the @"T:Newtonsoft.Json.Serialization.IValueProvider" used by the serializer to get and set values from a member.

|Parameter Name|Remarks|
|--------------|-------|
|member|The member.|

_returns: The @"T:Newtonsoft.Json.Serialization.IValueProvider" used by the serializer to get and set values from a member._

#### CreateObjectContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateObjectContract(System.Type)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonObjectContract" for the given type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: A @"T:Newtonsoft.Json.Serialization.JsonObjectContract" for the given type._

#### CreatePrimitiveContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreatePrimitiveContract(System.Type)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonPrimitiveContract" for the given type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: A @"T:Newtonsoft.Json.Serialization.JsonPrimitiveContract" for the given type._

#### CreateProperties
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateProperties(System.Type,Newtonsoft.Json.MemberSerialization)
```
Creates properties for the given @"T:Newtonsoft.Json.Serialization.JsonContract".

|Parameter Name|Remarks|
|--------------|-------|
|type|The type to create properties for.|
|memberSerialization|The member serialization mode for the type.|

_returns: Properties for the given @"T:Newtonsoft.Json.Serialization.JsonContract"._

#### CreateProperty
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateProperty(System.Reflection.MemberInfo,Newtonsoft.Json.MemberSerialization)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonProperty" for the given @"T:System.Reflection.MemberInfo".

|Parameter Name|Remarks|
|--------------|-------|
|memberSerialization|The member's parent @"T:Newtonsoft.Json.MemberSerialization".|
|member|The member to create a @"T:Newtonsoft.Json.Serialization.JsonProperty" for.|

_returns: A created @"T:Newtonsoft.Json.Serialization.JsonProperty" for the given @"T:System.Reflection.MemberInfo"._

#### CreatePropertyFromConstructorParameter
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreatePropertyFromConstructorParameter(Newtonsoft.Json.Serialization.JsonProperty,System.Reflection.ParameterInfo)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonProperty" for the given @"T:System.Reflection.ParameterInfo".

|Parameter Name|Remarks|
|--------------|-------|
|matchingMemberProperty|The matching member property.|
|parameterInfo|The constructor parameter.|

_returns: A created @"T:Newtonsoft.Json.Serialization.JsonProperty" for the given @"T:System.Reflection.ParameterInfo"._

#### CreateStringContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.CreateStringContract(System.Type)
```
Creates a @"T:Newtonsoft.Json.Serialization.JsonStringContract" for the given type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: A @"T:Newtonsoft.Json.Serialization.JsonStringContract" for the given type._

#### GetResolvedPropertyName
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.GetResolvedPropertyName(System.String)
```
Gets the resolved name of the property.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|

_returns: Name of the property._

#### GetSerializableMembers
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.GetSerializableMembers(System.Type)
```
Gets the serializable members for the type.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|The type to get serializable members for.|

_returns: The serializable members for the type._

#### ResolveContract
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.ResolveContract(System.Type)
```
Resolves the contract for a given type.

|Parameter Name|Remarks|
|--------------|-------|
|type|The type to resolve a contract for.|

_returns: The contract for a given type._

#### ResolveContractConverter
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.ResolveContractConverter(System.Type)
```
Resolves the default for the contract.

|Parameter Name|Remarks|
|--------------|-------|
|objectType|Type of the object.|

_returns: The contract's default ._

#### ResolveDictionaryKey
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.ResolveDictionaryKey(System.String)
```
Resolves the key of the dictionary. By default @"M:Newtonsoft.Json.Serialization.DefaultContractResolver.ResolvePropertyName(System.String)" is used to resolve dictionary keys.

|Parameter Name|Remarks|
|--------------|-------|
|dictionaryKey|Key of the dictionary.|

_returns: Resolved key of the dictionary._

#### ResolvePropertyName
```csharp
Newtonsoft.Json.Serialization.DefaultContractResolver.ResolvePropertyName(System.String)
```
Resolves the name of the property.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|

_returns: Resolved name of the property._


### Properties

#### DefaultMembersSearchFlags
Gets or sets the default members search flags.
#### DynamicCodeGeneration
Gets a value indicating whether members are being get and set using dynamic code generation.
 This value is determined by the runtime permissions available.
#### IgnoreSerializableAttribute
Gets or sets a value indicating whether to ignore the @"T:System.SerializableAttribute" attribute when serializing and deserializing types.
#### IgnoreSerializableInterface
Gets or sets a value indicating whether to ignore the @"T:System.Runtime.Serialization.ISerializable" interface when serializing and deserializing types.
#### NamingStrategy
Gets or sets the naming strategy used to resolve how property names and dictionary keys are serialized.
#### SerializeCompilerGeneratedMembers
Gets or sets a value indicating whether compiler generated members should be serialized.
