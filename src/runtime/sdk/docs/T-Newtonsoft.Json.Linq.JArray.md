---
title: JArray
---

# JArray
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents a JSON array.



### Methods

#### #ctor
```csharp
Newtonsoft.Json.Linq.JArray.#ctor(System.Object)
```
Initializes a new instance of the @"T:Newtonsoft.Json.Linq.JArray" class with the specified content.

|Parameter Name|Remarks|
|--------------|-------|
|content|The contents of the array.|


#### Add
```csharp
Newtonsoft.Json.Linq.JArray.Add(Newtonsoft.Json.Linq.JToken)
```
Adds an item to the @"T:System.Collections.Generic.ICollection`1".

|Parameter Name|Remarks|
|--------------|-------|
|item|The object to add to the @"T:System.Collections.Generic.ICollection`1".|


#### Clear
```csharp
Newtonsoft.Json.Linq.JArray.Clear
```
Removes all items from the @"T:System.Collections.Generic.ICollection`1".

#### Contains
```csharp
Newtonsoft.Json.Linq.JArray.Contains(Newtonsoft.Json.Linq.JToken)
```
Determines whether the @"T:System.Collections.Generic.ICollection`1" contains a specific value.

|Parameter Name|Remarks|
|--------------|-------|
|item|The object to locate in the @"T:System.Collections.Generic.ICollection`1".|

_returns: 
            true if **item** is found in the @"T:System.Collections.Generic.ICollection`1"; otherwise, false.
            _

#### CopyTo
```csharp
Newtonsoft.Json.Linq.JArray.CopyTo(Newtonsoft.Json.Linq.JToken[],System.Int32)
```
Copies to.

|Parameter Name|Remarks|
|--------------|-------|
|array|The array.|
|arrayIndex|Index of the array.|


#### FromObject
```csharp
Newtonsoft.Json.Linq.JArray.FromObject(System.Object,Newtonsoft.Json.JsonSerializer)
```
Creates a @"T:Newtonsoft.Json.Linq.JArray" from an object.

|Parameter Name|Remarks|
|--------------|-------|
|o|The object that will be used to create @"T:Newtonsoft.Json.Linq.JArray".|
|jsonSerializer|The @"T:Newtonsoft.Json.JsonSerializer" that will be used to read the object.|

_returns: A @"T:Newtonsoft.Json.Linq.JArray" with the values of the specified object_

#### GetEnumerator
```csharp
Newtonsoft.Json.Linq.JArray.GetEnumerator
```
Returns an enumerator that iterates through the collection.
_returns: 
            A  that can be used to iterate through the collection.
            _

#### IndexOf
```csharp
Newtonsoft.Json.Linq.JArray.IndexOf(Newtonsoft.Json.Linq.JToken)
```
Determines the index of a specific item in the @"T:System.Collections.Generic.IList`1".

|Parameter Name|Remarks|
|--------------|-------|
|item|The object to locate in the @"T:System.Collections.Generic.IList`1".|

_returns: 
            The index of **item** if found in the list; otherwise, -1.
            _

#### Insert
```csharp
Newtonsoft.Json.Linq.JArray.Insert(System.Int32,Newtonsoft.Json.Linq.JToken)
```
Inserts an item to the @"T:System.Collections.Generic.IList`1" at the specified index.

|Parameter Name|Remarks|
|--------------|-------|
|index|The zero-based index at which **item** should be inserted.|
|item|The object to insert into the @"T:System.Collections.Generic.IList`1".|


#### Load
```csharp
Newtonsoft.Json.Linq.JArray.Load(Newtonsoft.Json.JsonReader,Newtonsoft.Json.Linq.JsonLoadSettings)
```
Loads an @"T:Newtonsoft.Json.Linq.JArray" from a @"T:Newtonsoft.Json.JsonReader".

|Parameter Name|Remarks|
|--------------|-------|
|reader|A @"T:Newtonsoft.Json.JsonReader" that will be read for the content of the @"T:Newtonsoft.Json.Linq.JArray".|
|settings|The @"T:Newtonsoft.Json.Linq.JsonLoadSettings" used to load the JSON.
            If this is null, default load settings will be used.|

_returns: A @"T:Newtonsoft.Json.Linq.JArray" that contains the JSON that was read from the specified @"T:Newtonsoft.Json.JsonReader"._

#### Parse
```csharp
Newtonsoft.Json.Linq.JArray.Parse(System.String,Newtonsoft.Json.Linq.JsonLoadSettings)
```
Load a @"T:Newtonsoft.Json.Linq.JArray" from a string that contains JSON.

|Parameter Name|Remarks|
|--------------|-------|
|json|A @"T:System.String" that contains JSON.|
|settings|The @"T:Newtonsoft.Json.Linq.JsonLoadSettings" used to load the JSON.
            If this is null, default load settings will be used.|

_returns: A @"T:Newtonsoft.Json.Linq.JArray" populated from the string that contains JSON._

#### Remove
```csharp
Newtonsoft.Json.Linq.JArray.Remove(Newtonsoft.Json.Linq.JToken)
```
Removes the first occurrence of a specific object from the @"T:System.Collections.Generic.ICollection`1".

|Parameter Name|Remarks|
|--------------|-------|
|item|The object to remove from the @"T:System.Collections.Generic.ICollection`1".|

_returns: 
            true if **item** was successfully removed from the @"T:System.Collections.Generic.ICollection`1"; otherwise, false. This method also returns false if **item** is not found in the original @"T:System.Collections.Generic.ICollection`1".
            _

#### RemoveAt
```csharp
Newtonsoft.Json.Linq.JArray.RemoveAt(System.Int32)
```
Removes the @"T:System.Collections.Generic.IList`1" item at the specified index.

|Parameter Name|Remarks|
|--------------|-------|
|index|The zero-based index of the item to remove.|


#### WriteTo
```csharp
Newtonsoft.Json.Linq.JArray.WriteTo(Newtonsoft.Json.JsonWriter,Newtonsoft.Json.JsonConverter[])
```
Writes this token to a @"T:Newtonsoft.Json.JsonWriter".

|Parameter Name|Remarks|
|--------------|-------|
|writer|A @"T:Newtonsoft.Json.JsonWriter" into which this method will write.|
|converters|A collection of @"T:Newtonsoft.Json.JsonConverter" which will be used when writing the token.|



### Properties

#### ChildrenTokens
Gets the container's children tokens.
#### IsReadOnly
Gets a value indicating whether the is read-only.
#### Item
Gets or sets the @"T:Newtonsoft.Json.Linq.JToken" at the specified index.
#### Type
Gets the node type for this @"T:Newtonsoft.Json.Linq.JToken".
