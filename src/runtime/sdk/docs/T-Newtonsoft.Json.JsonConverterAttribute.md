---
title: JsonConverterAttribute
---

# JsonConverterAttribute
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Instructs the @"T:Newtonsoft.Json.JsonSerializer" to use the specified @"T:Newtonsoft.Json.JsonConverter" when serializing the member or class.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.JsonConverterAttribute.#ctor(System.Type,System.Object[])
```
Initializes a new instance of the @"T:Newtonsoft.Json.JsonConverterAttribute" class.

|Parameter Name|Remarks|
|--------------|-------|
|converterType|Type of the @"T:Newtonsoft.Json.JsonConverter".|
|converterParameters|Parameter list to use when constructing the @"T:Newtonsoft.Json.JsonConverter". Can be null.|



### Properties

#### ConverterParameters
The parameter list to use when constructing the @"T:Newtonsoft.Json.JsonConverter" described by ConverterType. 
 If null, the default constructor is used.
#### ConverterType
Gets the @"T:System.Type" of the @"T:Newtonsoft.Json.JsonConverter".
