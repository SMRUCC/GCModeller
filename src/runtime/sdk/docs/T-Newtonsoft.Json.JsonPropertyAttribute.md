---
title: JsonPropertyAttribute
---

# JsonPropertyAttribute
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Instructs the @"T:Newtonsoft.Json.JsonSerializer" to always serialize the member with the specified name.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonPropertyAttribute.#ctor(System.String)
```
Initializes a new instance of the @"T:Newtonsoft.Json.JsonPropertyAttribute" class with the specified name.

|Parameter Name|Remarks|
|--------------|-------|
|propertyName|Name of the property.|



### Properties

#### DefaultValueHandling
Gets or sets the default value handling used when serializing this property.
#### IsReference
Gets or sets whether this property's value is serialized as a reference.
#### ItemConverterParameters
The parameter list to use when constructing the @"T:Newtonsoft.Json.JsonConverter" described by ItemConverterType.
 If null, the default constructor is used.
 When non-null, there must be a constructor defined in the @"T:Newtonsoft.Json.JsonConverter" that exactly matches the number,
 order, and type of these parameters.
#### ItemConverterType
Gets or sets the @"T:Newtonsoft.Json.JsonConverter" used when serializing the property's collection items.
#### ItemIsReference
Gets or sets whether this property's collection items are serialized as a reference.
#### ItemReferenceLoopHandling
Gets or sets the the reference loop handling used when serializing the property's collection items.
#### ItemTypeNameHandling
Gets or sets the the type name handling used when serializing the property's collection items.
#### NamingStrategyParameters
The parameter list to use when constructing the @"T:Newtonsoft.Json.Serialization.NamingStrategy" described by NamingStrategyType. 
 If null, the default constructor is used.
 When non-null, there must be a constructor defined in the @"T:Newtonsoft.Json.Serialization.NamingStrategy" that exactly matches the number,
 order, and type of these parameters.
#### NamingStrategyType
Gets or sets the @"T:System.Type" of the @"T:Newtonsoft.Json.Serialization.NamingStrategy".
#### NullValueHandling
Gets or sets the null value handling used when serializing this property.
#### ObjectCreationHandling
Gets or sets the object creation handling used when deserializing this property.
#### Order
Gets or sets the order of serialization of a member.
#### PropertyName
Gets or sets the name of the property.
#### ReferenceLoopHandling
Gets or sets the reference loop handling used when serializing this property.
#### Required
Gets or sets a value indicating whether this property is required.
#### TypeNameHandling
Gets or sets the type name handling used when serializing this property.
