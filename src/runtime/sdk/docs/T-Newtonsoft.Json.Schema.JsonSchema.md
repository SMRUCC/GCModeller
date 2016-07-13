---
title: JsonSchema
---

# JsonSchema
_namespace: [Newtonsoft.Json.Schema](N-Newtonsoft.Json.Schema.html)_

An in-memory representation of a JSON Schema.
 
 JSON Schema validation has been moved to its own package. See http://www.newtonsoft.com/jsonschema for more details.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Schema.JsonSchema.#ctor
```
Initializes a new instance of the @"T:Newtonsoft.Json.Schema.JsonSchema" class.

#### Parse
```csharp
Newtonsoft.Json.Schema.JsonSchema.Parse(System.String,Newtonsoft.Json.Schema.JsonSchemaResolver)
```
Parses the specified json.

|Parameter Name|Remarks|
|--------------|-------|
|json|The json.|
|resolver|The resolver.|

_returns: A @"T:Newtonsoft.Json.Schema.JsonSchema" populated from the string that contains JSON._

#### Read
```csharp
Newtonsoft.Json.Schema.JsonSchema.Read(Newtonsoft.Json.JsonReader,Newtonsoft.Json.Schema.JsonSchemaResolver)
```
Reads a @"T:Newtonsoft.Json.Schema.JsonSchema" from the specified @"T:Newtonsoft.Json.JsonReader".

|Parameter Name|Remarks|
|--------------|-------|
|reader|The @"T:Newtonsoft.Json.JsonReader" containing the JSON Schema to read.|
|resolver|The @"T:Newtonsoft.Json.Schema.JsonSchemaResolver" to use when resolving schema references.|

_returns: The @"T:Newtonsoft.Json.Schema.JsonSchema" object representing the JSON Schema._

#### ToString
```csharp
Newtonsoft.Json.Schema.JsonSchema.ToString
```
Returns a @"T:System.String" that represents the current @"T:System.Object".
_returns: 
            A @"T:System.String" that represents the current @"T:System.Object".
            _

#### WriteTo
```csharp
Newtonsoft.Json.Schema.JsonSchema.WriteTo(Newtonsoft.Json.JsonWriter,Newtonsoft.Json.Schema.JsonSchemaResolver)
```
Writes this schema to a @"T:Newtonsoft.Json.JsonWriter" using the specified @"T:Newtonsoft.Json.Schema.JsonSchemaResolver".

|Parameter Name|Remarks|
|--------------|-------|
|writer|A @"T:Newtonsoft.Json.JsonWriter" into which this method will write.|
|resolver|The resolver used.|



### Properties

#### AdditionalItems
Gets or sets the @"T:Newtonsoft.Json.Schema.JsonSchema" of additional items.
#### AdditionalProperties
Gets or sets the @"T:Newtonsoft.Json.Schema.JsonSchema" of additional properties.
#### AllowAdditionalItems
Gets or sets a value indicating whether additional items are allowed.
#### AllowAdditionalProperties
Gets or sets a value indicating whether additional properties are allowed.
#### Default
Gets or sets the default value.
#### Description
Gets or sets the description of the object.
#### Disallow
Gets or sets disallowed types.
#### DivisibleBy
Gets or sets a number that the value should be divisble by.
#### Enum
Gets or sets the a collection of valid enum values allowed.
#### ExclusiveMaximum
Gets or sets a flag indicating whether the value can not equal the number defined by the "maximum" attribute.
#### ExclusiveMinimum
Gets or sets a flag indicating whether the value can not equal the number defined by the "minimum" attribute.
#### Extends
Gets or sets the collection of @"T:Newtonsoft.Json.Schema.JsonSchema" that this schema extends.
#### Format
Gets or sets the format.
#### Hidden
Gets or sets whether the object is visible to users.
#### Id
Gets or sets the id.
#### Items
Gets or sets the @"T:Newtonsoft.Json.Schema.JsonSchema" of items.
#### Maximum
Gets or sets the maximum.
#### MaximumItems
Gets or sets the maximum number of items.
#### MaximumLength
Gets or sets the maximum length.
#### Minimum
Gets or sets the minimum.
#### MinimumItems
Gets or sets the minimum number of items.
#### MinimumLength
Gets or sets the minimum length.
#### Pattern
Gets or sets the pattern.
#### PatternProperties
Gets or sets the pattern properties.
#### PositionalItemsValidation
Gets or sets a value indicating whether items in an array are validated using the @"T:Newtonsoft.Json.Schema.JsonSchema" instance at their array position from @"P:Newtonsoft.Json.Schema.JsonSchema.Items".
#### Properties
Gets or sets the @"T:Newtonsoft.Json.Schema.JsonSchema" of properties.
#### ReadOnly
Gets or sets whether the object is read only.
#### Required
Gets or sets whether the object is required.
#### Requires
Gets or sets the required property if this property is present.
#### Title
Gets or sets the title.
#### Transient
Gets or sets whether the object is transient.
#### Type
Gets or sets the types of values allowed by the object.
#### UniqueItems
Gets or sets whether the array items must be unique.
