---
title: JValue
---

# JValue
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents a value in JSON (string, integer, date, etc).



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Linq.JValue.#ctor(System.Object)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Linq.JValue" class with the given value.

|Parameter Name|Remarks|
|--------------|-------|
|value|The value.|


#### CompareTo
```csharp
Newtonsoft.Json.Linq.JValue.CompareTo(Newtonsoft.Json.Linq.JValue)
```
Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.

|Parameter Name|Remarks|
|--------------|-------|
|obj|An object to compare with this instance.|

_returns: 
            A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings:
            Value
            Meaning
            Less than zero
            This instance is less than **obj**.
            Zero
            This instance is equal to **obj**.
            Greater than zero
            This instance is greater than **obj**.
            _

#### CreateComment
```csharp
Newtonsoft.Json.Linq.JValue.CreateComment(System.String)
```
Creates a @"T:Newtonsoft.Json.Linq.JValue" comment with the given value.

|Parameter Name|Remarks|
|--------------|-------|
|value|The value.|

_returns: A @"T:Newtonsoft.Json.Linq.JValue" comment with the given value._

#### CreateNull
```csharp
Newtonsoft.Json.Linq.JValue.CreateNull
```
Creates a @"T:Newtonsoft.Json.Linq.JValue" null value.
_returns: A @"T:Newtonsoft.Json.Linq.JValue" null value._

#### CreateString
```csharp
Newtonsoft.Json.Linq.JValue.CreateString(System.String)
```
Creates a @"T:Newtonsoft.Json.Linq.JValue" string with the given value.

|Parameter Name|Remarks|
|--------------|-------|
|value|The value.|

_returns: A @"T:Newtonsoft.Json.Linq.JValue" string with the given value._

#### CreateUndefined
```csharp
Newtonsoft.Json.Linq.JValue.CreateUndefined
```
Creates a @"T:Newtonsoft.Json.Linq.JValue" undefined value.
_returns: A @"T:Newtonsoft.Json.Linq.JValue" undefined value._

#### Equals
```csharp
Newtonsoft.Json.Linq.JValue.Equals(System.Object)
```
Determines whether the specified @"T:System.Object" is equal to the current @"T:System.Object".

|Parameter Name|Remarks|
|--------------|-------|
|obj|The @"T:System.Object" to compare with the current @"T:System.Object".|

_returns: true if the specified @"T:System.Object" is equal to the current @"T:System.Object"; otherwise, false.
            _

#### GetHashCode
```csharp
Newtonsoft.Json.Linq.JValue.GetHashCode
```
Serves as a hash function for a particular type.
_returns: 
            A hash code for the current @"T:System.Object".
            _

#### GetMetaObject
```csharp
Newtonsoft.Json.Linq.JValue.GetMetaObject(System.Linq.Expressions.Expression)
```
Returns the @"T:System.Dynamic.DynamicMetaObject" responsible for binding operations performed on this object.

|Parameter Name|Remarks|
|--------------|-------|
|parameter|The expression tree representation of the runtime value.|

_returns: 
            The @"T:System.Dynamic.DynamicMetaObject" to bind this object.
            _

#### ToString
```csharp
Newtonsoft.Json.Linq.JValue.ToString(System.String,System.IFormatProvider)
```
Returns a @"T:System.String" that represents this instance.

|Parameter Name|Remarks|
|--------------|-------|
|format|The format.|
|formatProvider|The format provider.|

_returns: 
            A @"T:System.String" that represents this instance.
            _

#### WriteTo
```csharp
Newtonsoft.Json.Linq.JValue.WriteTo(Newtonsoft.Json.JsonWriter,Newtonsoft.Json.JsonConverter[])
```
Writes this token to a @"T:Newtonsoft.Json.JsonWriter".

|Parameter Name|Remarks|
|--------------|-------|
|writer|A @"T:Newtonsoft.Json.JsonWriter" into which this method will write.|
|converters|A collection of @"T:Newtonsoft.Json.JsonConverter" which will be used when writing the token.|



### Properties

#### HasValues
Gets a value indicating whether this token has child tokens.
#### Type
Gets the node type for this @"T:Newtonsoft.Json.Linq.JToken".
#### Value
Gets or sets the underlying token value.
