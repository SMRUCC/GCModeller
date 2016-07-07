---
title: JsonSchemaResolver
---

# JsonSchemaResolver
_namespace: [Newtonsoft.Json.Schema](N-Newtonsoft.Json.Schema.html)_

Resolves @"T:Newtonsoft.Json.Schema.JsonSchema" from an id.
 
 JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Schema.JsonSchemaResolver.#ctor
```
Initializes a new instance of the @"T:Newtonsoft.Json.Schema.JsonSchemaResolver" class.

#### GetSchema
```csharp
Newtonsoft.Json.Schema.JsonSchemaResolver.GetSchema(System.String)
```
Gets a @"T:Newtonsoft.Json.Schema.JsonSchema" for the specified reference.

|Parameter Name|Remarks|
|--------------|-------|
|reference|The id.|

_returns: A @"T:Newtonsoft.Json.Schema.JsonSchema" for the specified reference._


### Properties

#### LoadedSchemas
Gets or sets the loaded schemas.
