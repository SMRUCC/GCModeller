---
title: JContainer
---

# JContainer
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Represents a token that can contain other tokens.



### Methods

#### Add
```csharp
Newtonsoft.Json.Linq.JContainer.Add(System.Object)
```
Adds the specified content as children of this @"T:Newtonsoft.Json.Linq.JToken".

|Parameter Name|Remarks|
|--------------|-------|
|content|The content to be added.|


#### AddFirst
```csharp
Newtonsoft.Json.Linq.JContainer.AddFirst(System.Object)
```
Adds the specified content as the first children of this @"T:Newtonsoft.Json.Linq.JToken".

|Parameter Name|Remarks|
|--------------|-------|
|content|The content to be added.|


#### Children
```csharp
Newtonsoft.Json.Linq.JContainer.Children
```
Returns a collection of the child tokens of this token, in document order.
_returns: 
            An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" containing the child tokens of this @"T:Newtonsoft.Json.Linq.JToken", in document order.
            _

#### CreateWriter
```csharp
Newtonsoft.Json.Linq.JContainer.CreateWriter
```
Creates an @"T:Newtonsoft.Json.JsonWriter" that can be used to add tokens to the @"T:Newtonsoft.Json.Linq.JToken".
_returns: An @"T:Newtonsoft.Json.JsonWriter" that is ready to have content written to it._

#### Descendants
```csharp
Newtonsoft.Json.Linq.JContainer.Descendants
```
Returns a collection of the descendant tokens for this token in document order.
_returns: An @"T:System.Collections.Generic.IEnumerable`1" containing the descendant tokens of the @"T:Newtonsoft.Json.Linq.JToken"._

#### DescendantsAndSelf
```csharp
Newtonsoft.Json.Linq.JContainer.DescendantsAndSelf
```
Returns a collection of the tokens that contain this token, and all descendant tokens of this token, in document order.
_returns: An @"T:System.Collections.Generic.IEnumerable`1" containing this token, and all the descendant tokens of the @"T:Newtonsoft.Json.Linq.JToken"._

#### Merge
```csharp
Newtonsoft.Json.Linq.JContainer.Merge(System.Object,Newtonsoft.Json.Linq.JsonMergeSettings)
```
Merge the specified content into this @"T:Newtonsoft.Json.Linq.JToken" using @"T:Newtonsoft.Json.Linq.JsonMergeSettings".

|Parameter Name|Remarks|
|--------------|-------|
|content|The content to be merged.|
|settings|The @"T:Newtonsoft.Json.Linq.JsonMergeSettings" used to merge the content.|


#### OnAddingNew
```csharp
Newtonsoft.Json.Linq.JContainer.OnAddingNew(System.ComponentModel.AddingNewEventArgs)
```
Raises the @"E:Newtonsoft.Json.Linq.JContainer.AddingNew" event.

|Parameter Name|Remarks|
|--------------|-------|
|e|The @"T:System.ComponentModel.AddingNewEventArgs" instance containing the event data.|


#### OnCollectionChanged
```csharp
Newtonsoft.Json.Linq.JContainer.OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs)
```
Raises the @"E:Newtonsoft.Json.Linq.JContainer.CollectionChanged" event.

|Parameter Name|Remarks|
|--------------|-------|
|e|The @"T:System.Collections.Specialized.NotifyCollectionChangedEventArgs" instance containing the event data.|


#### OnListChanged
```csharp
Newtonsoft.Json.Linq.JContainer.OnListChanged(System.ComponentModel.ListChangedEventArgs)
```
Raises the @"E:Newtonsoft.Json.Linq.JContainer.ListChanged" event.

|Parameter Name|Remarks|
|--------------|-------|
|e|The @"T:System.ComponentModel.ListChangedEventArgs" instance containing the event data.|


#### RemoveAll
```csharp
Newtonsoft.Json.Linq.JContainer.RemoveAll
```
Removes the child nodes from this token.

#### ReplaceAll
```csharp
Newtonsoft.Json.Linq.JContainer.ReplaceAll(System.Object)
```
Replaces the children nodes of this token with the specified content.

|Parameter Name|Remarks|
|--------------|-------|
|content|The content.|


#### Values``1
```csharp
Newtonsoft.Json.Linq.JContainer.Values``1
```
Returns a collection of the child values of this token, in document order.
_returns: 
            A @"T:System.Collections.Generic.IEnumerable`1" containing the child values of this @"T:Newtonsoft.Json.Linq.JToken", in document order.
            _


### Properties

#### ChildrenTokens
Gets the container's children tokens.
#### Count
Gets the count of child JSON tokens.
#### First
Get the first child token of this token.
#### HasValues
Gets a value indicating whether this token has child tokens.
#### Last
Get the last child token of this token.
