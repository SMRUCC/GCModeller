---
title: JsonContainerAttribute
---

# JsonContainerAttribute
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Instructs the @"T:Newtonsoft.Json.JsonSerializer" how to serialize the object.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonContainerAttribute.#ctor(System.String)
```
Initializes a new instance of the @"T:Newtonsoft.Json.JsonContainerAttribute" class with the specified container Id.

|Parameter Name|Remarks|
|--------------|-------|
|id|The container Id.|



### Properties

#### Description
Gets or sets the description.
#### Id
Gets or sets the id.
#### IsReference
Gets or sets a value that indicates whether to preserve object references.
#### ItemConverterParameters
The parameter list to use when constructing the @"T:Newtonsoft.Json.JsonConverter" described by ItemConverterType.
 If null, the default constructor is used.
 When non-null, there must be a constructor defined in the @"T:Newtonsoft.Json.JsonConverter" that exactly matches the number,
 order, and type of these parameters.
#### ItemConverterType
Gets or sets the collection's items converter.
#### ItemIsReference
Gets or sets a value that indicates whether to preserve collection's items references.
#### ItemReferenceLoopHandling
Gets or sets the reference loop handling used when serializing the collection's items.
#### ItemTypeNameHandling
Gets or sets the type name handling used when serializing the collection's items.
#### NamingStrategyParameters
The parameter list to use when constructing the @"T:Newtonsoft.Json.Serialization.NamingStrategy" described by NamingStrategyType. 
 If null, the default constructor is used.
 When non-null, there must be a constructor defined in the @"T:Newtonsoft.Json.Serialization.NamingStrategy" that exactly matches the number,
 order, and type of these parameters.
#### NamingStrategyType
Gets or sets the @"T:System.Type" of the @"T:Newtonsoft.Json.Serialization.NamingStrategy".
#### Title
Gets or sets the title.
