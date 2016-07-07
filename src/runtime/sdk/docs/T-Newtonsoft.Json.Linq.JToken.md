---
title: JToken
---

# JToken
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents an abstract JSON token.



### Methods

#### AddAfterSelf
```csharp
Newtonsoft.Json.Linq.JToken.AddAfterSelf(System.Object)
```
Adds the specified content immediately after this token.

|Parameter Name|Remarks|
|--------------|-------|
|content|A content object that contains simple content or a collection of content objects to be added after this token.|


#### AddAnnotation
```csharp
Newtonsoft.Json.Linq.JToken.AddAnnotation(System.Object)
```
Adds an object to the annotation list of this @"T:Newtonsoft.Json.Linq.JToken".

|Parameter Name|Remarks|
|--------------|-------|
|annotation|The annotation to add.|


#### AddBeforeSelf
```csharp
Newtonsoft.Json.Linq.JToken.AddBeforeSelf(System.Object)
```
Adds the specified content immediately before this token.

|Parameter Name|Remarks|
|--------------|-------|
|content|A content object that contains simple content or a collection of content objects to be added before this token.|


#### AfterSelf
```csharp
Newtonsoft.Json.Linq.JToken.AfterSelf
```
Returns a collection of the sibling tokens after this token, in document order.
_returns: A collection of the sibling tokens after this tokens, in document order._

#### Ancestors
```csharp
Newtonsoft.Json.Linq.JToken.Ancestors
```
Returns a collection of the ancestor tokens of this token.
_returns: A collection of the ancestor tokens of this token._

#### AncestorsAndSelf
```csharp
Newtonsoft.Json.Linq.JToken.AncestorsAndSelf
```
Returns a collection of tokens that contain this token, and the ancestors of this token.
_returns: A collection of tokens that contain this token, and the ancestors of this token._

#### Annotation
```csharp
Newtonsoft.Json.Linq.JToken.Annotation(System.Type)
```
Gets the first annotation object of the specified type from this @"T:Newtonsoft.Json.Linq.JToken".

|Parameter Name|Remarks|
|--------------|-------|
|type|The @"P:Newtonsoft.Json.Linq.JToken.Type" of the annotation to retrieve.|

_returns: The first annotation object that matches the specified type, or null if no annotation is of the specified type._

#### Annotation``1
```csharp
Newtonsoft.Json.Linq.JToken.Annotation``1
```
Get the first annotation object of the specified type from this @"T:Newtonsoft.Json.Linq.JToken".
_returns: The first annotation object that matches the specified type, or null if no annotation is of the specified type._

#### Annotations
```csharp
Newtonsoft.Json.Linq.JToken.Annotations(System.Type)
```
Gets a collection of annotations of the specified type for this @"T:Newtonsoft.Json.Linq.JToken".

|Parameter Name|Remarks|
|--------------|-------|
|type|The @"P:Newtonsoft.Json.Linq.JToken.Type" of the annotations to retrieve.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" of @"T:System.Object" that contains the annotations that match the specified type for this @"T:Newtonsoft.Json.Linq.JToken"._

#### Annotations``1
```csharp
Newtonsoft.Json.Linq.JToken.Annotations``1
```
Gets a collection of annotations of the specified type for this @"T:Newtonsoft.Json.Linq.JToken".
_returns: An @"T:System.Collections.Generic.IEnumerable`1"  that contains the annotations for this @"T:Newtonsoft.Json.Linq.JToken"._

#### BeforeSelf
```csharp
Newtonsoft.Json.Linq.JToken.BeforeSelf
```
Returns a collection of the sibling tokens before this token, in document order.
_returns: A collection of the sibling tokens before this token, in document order._

#### Children
```csharp
Newtonsoft.Json.Linq.JToken.Children
```
Returns a collection of the child tokens of this token, in document order.
_returns: An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" containing the child tokens of this @"T:Newtonsoft.Json.Linq.JToken", in document order._

#### Children``1
```csharp
Newtonsoft.Json.Linq.JToken.Children``1
```
Returns a collection of the child tokens of this token, in document order, filtered by the specified type.
_returns: A @"T:Newtonsoft.Json.Linq.JEnumerable`1" containing the child tokens of this @"T:Newtonsoft.Json.Linq.JToken", in document order._

#### CreateReader
```csharp
Newtonsoft.Json.Linq.JToken.CreateReader
```
Creates an @"T:Newtonsoft.Json.JsonReader" for this token.
_returns: An @"T:Newtonsoft.Json.JsonReader" that can be used to read this token and its descendants._

#### DeepClone
```csharp
Newtonsoft.Json.Linq.JToken.DeepClone
```
Creates a new instance of the @"T:Newtonsoft.Json.Linq.JToken". All child tokens are recursively cloned.
_returns: A new instance of the @"T:Newtonsoft.Json.Linq.JToken"._

#### DeepEquals
```csharp
Newtonsoft.Json.Linq.JToken.DeepEquals(Newtonsoft.Json.Linq.JToken,Newtonsoft.Json.Linq.JToken)
```
Compares the values of two tokens, including the values of all descendant tokens.

|Parameter Name|Remarks|
|--------------|-------|
|t1|The first @"T:Newtonsoft.Json.Linq.JToken" to compare.|
|t2|The second @"T:Newtonsoft.Json.Linq.JToken" to compare.|

_returns: true if the tokens are equal; otherwise false._

#### FromObject
```csharp
Newtonsoft.Json.Linq.JToken.FromObject(System.Object,Newtonsoft.Json.JsonSerializer)
```
Creates a @"T:Newtonsoft.Json.Linq.JToken" from an object using the specified @"T:Newtonsoft.Json.JsonSerializer".

|Parameter Name|Remarks|
|--------------|-------|
|o|The object that will be used to create @"T:Newtonsoft.Json.Linq.JToken".|
|jsonSerializer|The @"T:Newtonsoft.Json.JsonSerializer" that will be used when reading the object.|

_returns: A @"T:Newtonsoft.Json.Linq.JToken" with the value of the specified object_

#### GetMetaObject
```csharp
Newtonsoft.Json.Linq.JToken.GetMetaObject(System.Linq.Expressions.Expression)
```
Returns the @"T:System.Dynamic.DynamicMetaObject" responsible for binding operations performed on this object.

|Parameter Name|Remarks|
|--------------|-------|
|parameter|The expression tree representation of the runtime value.|

_returns: 
            The @"T:System.Dynamic.DynamicMetaObject" to bind this object.
            _

#### Load
```csharp
Newtonsoft.Json.Linq.JToken.Load(Newtonsoft.Json.JsonReader)
```
Creates a @"T:Newtonsoft.Json.Linq.JToken" from a @"T:Newtonsoft.Json.JsonReader".

|Parameter Name|Remarks|
|--------------|-------|
|reader|An @"T:Newtonsoft.Json.JsonReader" positioned at the token to read into this @"T:Newtonsoft.Json.Linq.JToken".|

_returns: 
            An @"T:Newtonsoft.Json.Linq.JToken" that contains the token and its descendant tokens
            that were read from the reader. The runtime type of the token is determined
            by the token type of the first token encountered in the reader.
            _

#### op_Explicit
```csharp
Newtonsoft.Json.Linq.JToken.op_Explicit(Newtonsoft.Json.Linq.JToken)~System.Uri
```
Performs an explicit conversion from @"T:Newtonsoft.Json.Linq.JToken" to @"T:System.Uri".

|Parameter Name|Remarks|
|--------------|-------|
|value|The value.|

_returns: The result of the conversion._

#### op_Implicit
```csharp
Newtonsoft.Json.Linq.JToken.op_Implicit(System.Nullable{System.Guid})~Newtonsoft.Json.Linq.JToken
```
Performs an implicit conversion from @"T:System.Nullable`1" to @"T:Newtonsoft.Json.Linq.JToken".

|Parameter Name|Remarks|
|--------------|-------|
|value|The value to create a @"T:Newtonsoft.Json.Linq.JValue" from.|

_returns: The @"T:Newtonsoft.Json.Linq.JValue" initialized with the specified value._

#### Parse
```csharp
Newtonsoft.Json.Linq.JToken.Parse(System.String,Newtonsoft.Json.Linq.JsonLoadSettings)
```
Load a @"T:Newtonsoft.Json.Linq.JToken" from a string that contains JSON.

|Parameter Name|Remarks|
|--------------|-------|
|json|A @"T:System.String" that contains JSON.|
|settings|The @"T:Newtonsoft.Json.Linq.JsonLoadSettings" used to load the JSON.
            If this is null, default load settings will be used.|

_returns: A @"T:Newtonsoft.Json.Linq.JToken" populated from the string that contains JSON._

#### ReadFrom
```csharp
Newtonsoft.Json.Linq.JToken.ReadFrom(Newtonsoft.Json.JsonReader,Newtonsoft.Json.Linq.JsonLoadSettings)
```
Creates a @"T:Newtonsoft.Json.Linq.JToken" from a @"T:Newtonsoft.Json.JsonReader".

|Parameter Name|Remarks|
|--------------|-------|
|reader|An @"T:Newtonsoft.Json.JsonReader" positioned at the token to read into this @"T:Newtonsoft.Json.Linq.JToken".|
|settings|The @"T:Newtonsoft.Json.Linq.JsonLoadSettings" used to load the JSON.
            If this is null, default load settings will be used.|

_returns: 
            An @"T:Newtonsoft.Json.Linq.JToken" that contains the token and its descendant tokens
            that were read from the reader. The runtime type of the token is determined
            by the token type of the first token encountered in the reader.
            _

#### Remove
```csharp
Newtonsoft.Json.Linq.JToken.Remove
```
Removes this token from its parent.

#### RemoveAnnotations
```csharp
Newtonsoft.Json.Linq.JToken.RemoveAnnotations(System.Type)
```
Removes the annotations of the specified type from this @"T:Newtonsoft.Json.Linq.JToken".

|Parameter Name|Remarks|
|--------------|-------|
|type|The @"P:Newtonsoft.Json.Linq.JToken.Type" of annotations to remove.|


#### RemoveAnnotations``1
```csharp
Newtonsoft.Json.Linq.JToken.RemoveAnnotations``1
```
Removes the annotations of the specified type from this @"T:Newtonsoft.Json.Linq.JToken".

#### Replace
```csharp
Newtonsoft.Json.Linq.JToken.Replace(Newtonsoft.Json.Linq.JToken)
```
Replaces this token with the specified token.

|Parameter Name|Remarks|
|--------------|-------|
|value|The value.|


#### SelectToken
```csharp
Newtonsoft.Json.Linq.JToken.SelectToken(System.String,System.Boolean)
```
Selects a @"T:Newtonsoft.Json.Linq.JToken" using a JPath expression. Selects the token that matches the object path.

|Parameter Name|Remarks|
|--------------|-------|
|path|
            A @"T:System.String" that contains a JPath expression.
            |
|errorWhenNoMatch|A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.|

_returns: A @"T:Newtonsoft.Json.Linq.JToken"._

#### SelectTokens
```csharp
Newtonsoft.Json.Linq.JToken.SelectTokens(System.String,System.Boolean)
```
Selects a collection of elements using a JPath expression.

|Parameter Name|Remarks|
|--------------|-------|
|path|
            A @"T:System.String" that contains a JPath expression.
            |
|errorWhenNoMatch|A flag to indicate whether an error should be thrown if no tokens are found when evaluating part of the expression.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" that contains the selected elements._

#### System#Dynamic#IDynamicMetaObjectProvider#GetMetaObject
```csharp
Newtonsoft.Json.Linq.JToken.System#Dynamic#IDynamicMetaObjectProvider#GetMetaObject(System.Linq.Expressions.Expression)
```
Returns the @"T:System.Dynamic.DynamicMetaObject" responsible for binding operations performed on this object.

|Parameter Name|Remarks|
|--------------|-------|
|parameter|The expression tree representation of the runtime value.|

_returns: 
            The @"T:System.Dynamic.DynamicMetaObject" to bind this object.
            _

#### ToObject
```csharp
Newtonsoft.Json.Linq.JToken.ToObject(System.Type,Newtonsoft.Json.JsonSerializer)
```
Creates the specified .NET type from the @"T:Newtonsoft.Json.Linq.JToken" using the specified @"T:Newtonsoft.Json.JsonSerializer".

|Parameter Name|Remarks|
|--------------|-------|
|objectType|The object type that the token will be deserialized to.|
|jsonSerializer|The @"T:Newtonsoft.Json.JsonSerializer" that will be used when creating the object.|

_returns: The new object created from the JSON value._

#### ToObject``1
```csharp
Newtonsoft.Json.Linq.JToken.ToObject``1(Newtonsoft.Json.JsonSerializer)
```
Creates the specified .NET type from the @"T:Newtonsoft.Json.Linq.JToken" using the specified @"T:Newtonsoft.Json.JsonSerializer".

|Parameter Name|Remarks|
|--------------|-------|
|jsonSerializer|The @"T:Newtonsoft.Json.JsonSerializer" that will be used when creating the object.|

_returns: The new object created from the JSON value._

#### ToString
```csharp
Newtonsoft.Json.Linq.JToken.ToString(Newtonsoft.Json.Formatting,Newtonsoft.Json.JsonConverter[])
```
Returns the JSON for this token using the given formatting and converters.

|Parameter Name|Remarks|
|--------------|-------|
|formatting|Indicates how the output is formatted.|
|converters|A collection of @"T:Newtonsoft.Json.JsonConverter" which will be used when writing the token.|

_returns: The JSON for this token using the given formatting and converters._

#### Value``1
```csharp
Newtonsoft.Json.Linq.JToken.Value``1(System.Object)
```
Gets the @"T:Newtonsoft.Json.Linq.JToken" with the specified key converted to the specified type.

|Parameter Name|Remarks|
|--------------|-------|
|key|The token key.|

_returns: The converted token value._

#### Values``1
```csharp
Newtonsoft.Json.Linq.JToken.Values``1
```
Returns a collection of the child values of this token, in document order.
_returns: A @"T:System.Collections.Generic.IEnumerable`1" containing the child values of this @"T:Newtonsoft.Json.Linq.JToken", in document order._

#### WriteTo
```csharp
Newtonsoft.Json.Linq.JToken.WriteTo(Newtonsoft.Json.JsonWriter,Newtonsoft.Json.JsonConverter[])
```
Writes this token to a @"T:Newtonsoft.Json.JsonWriter".

|Parameter Name|Remarks|
|--------------|-------|
|writer|A @"T:Newtonsoft.Json.JsonWriter" into which this method will write.|
|converters|A collection of @"T:Newtonsoft.Json.JsonConverter" which will be used when writing the token.|



### Properties

#### EqualityComparer
Gets a comparer that can compare two tokens for value equality.
#### First
Get the first child token of this token.
#### HasValues
Gets a value indicating whether this token has child tokens.
#### Item
Gets the @"T:Newtonsoft.Json.Linq.JToken" with the specified key.
#### Last
Get the last child token of this token.
#### Next
Gets the next sibling token of this node.
#### Parent
Gets or sets the parent.
#### Path
Gets the path of the JSON token.
#### Previous
Gets the previous sibling token of this node.
#### Root
Gets the root @"T:Newtonsoft.Json.Linq.JToken" of this @"T:Newtonsoft.Json.Linq.JToken".
#### Type
Gets the node type for this @"T:Newtonsoft.Json.Linq.JToken".
