---
title: JsonSchemaGenerator
---

# JsonSchemaGenerator
_namespace: [Newtonsoft.Json.Schema](N-Newtonsoft.Json.Schema.html)_

Generates a @"T:Newtonsoft.Json.Schema.JsonSchema" from a specified @"T:System.Type".
 
 JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.



### Methods

#### Generate
```csharp
Newtonsoft.Json.Schema.JsonSchemaGenerator.Generate(System.Type,Newtonsoft.Json.Schema.JsonSchemaResolver,System.Boolean)
```
Generate a @"T:Newtonsoft.Json.Schema.JsonSchema" from the specified type.

|Parameter Name|Remarks|
|--------------|-------|
|type|The type to generate a @"T:Newtonsoft.Json.Schema.JsonSchema" from.|
|resolver|The @"T:Newtonsoft.Json.Schema.JsonSchemaResolver" used to resolve schema references.|
|rootSchemaNullable|Specify whether the generated root @"T:Newtonsoft.Json.Schema.JsonSchema" will be nullable.|

_returns: A @"T:Newtonsoft.Json.Schema.JsonSchema" generated from the specified type._


### Properties

#### ContractResolver
Gets or sets the contract resolver.
#### UndefinedSchemaIdHandling
Gets or sets how undefined schemas are handled by the serializer.
