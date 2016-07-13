---
title: Extensions
---

# Extensions
_namespace: [Newtonsoft.Json.Linq](N-Newtonsoft.Json.Linq.html)_

Contains the LINQ to JSON extension methods.



### Methods

#### Ancestors``1
```csharp
Newtonsoft.Json.Linq.Extensions.Ancestors``1(System.Collections.Generic.IEnumerable{``0})
```
Returns a collection of tokens that contains the ancestors of every token in the source collection.

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the ancestors of every token in the source collection._

#### AncestorsAndSelf``1
```csharp
Newtonsoft.Json.Linq.Extensions.AncestorsAndSelf``1(System.Collections.Generic.IEnumerable{``0})
```
Returns a collection of tokens that contains every token in the source collection, and the ancestors of every token in the source collection.

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains every token in the source collection, the ancestors of every token in the source collection._

#### AsJEnumerable
```csharp
Newtonsoft.Json.Linq.Extensions.AsJEnumerable(System.Collections.Generic.IEnumerable{Newtonsoft.Json.Linq.JToken})
```
Returns the input typed as @"T:Newtonsoft.Json.Linq.IJEnumerable`1".

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: The input typed as @"T:Newtonsoft.Json.Linq.IJEnumerable`1"._

#### AsJEnumerable``1
```csharp
Newtonsoft.Json.Linq.Extensions.AsJEnumerable``1(System.Collections.Generic.IEnumerable{``0})
```
Returns the input typed as @"T:Newtonsoft.Json.Linq.IJEnumerable`1".

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: The input typed as @"T:Newtonsoft.Json.Linq.IJEnumerable`1"._

#### Children``1
```csharp
Newtonsoft.Json.Linq.Extensions.Children``1(System.Collections.Generic.IEnumerable{``0})
```
Returns a collection of child tokens of every array in the source collection.

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the values of every token in the source collection._

#### Children``2
```csharp
Newtonsoft.Json.Linq.Extensions.Children``2(System.Collections.Generic.IEnumerable{``0})
```
Returns a collection of converted child tokens of every array in the source collection.

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" that contains the converted values of every token in the source collection._

#### Descendants``1
```csharp
Newtonsoft.Json.Linq.Extensions.Descendants``1(System.Collections.Generic.IEnumerable{``0})
```
Returns a collection of tokens that contains the descendants of every token in the source collection.

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the descendants of every token in the source collection._

#### DescendantsAndSelf``1
```csharp
Newtonsoft.Json.Linq.Extensions.DescendantsAndSelf``1(System.Collections.Generic.IEnumerable{``0})
```
Returns a collection of tokens that contains every token in the source collection, and the descendants of every token in the source collection.

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains every token in the source collection, and the descendants of every token in the source collection._

#### Properties
```csharp
Newtonsoft.Json.Linq.Extensions.Properties(System.Collections.Generic.IEnumerable{Newtonsoft.Json.Linq.JObject})
```
Returns a collection of child properties of every object in the source collection.

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JObject" that contains the source collection.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JProperty" that contains the properties of every object in the source collection._

#### Value``1
```csharp
Newtonsoft.Json.Linq.Extensions.Value``1(System.Collections.Generic.IEnumerable{Newtonsoft.Json.Linq.JToken})
```
Converts the value.

|Parameter Name|Remarks|
|--------------|-------|
|value|A @"T:Newtonsoft.Json.Linq.JToken" cast as a @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken".|

_returns: A converted value._

#### Value``2
```csharp
Newtonsoft.Json.Linq.Extensions.Value``2(System.Collections.Generic.IEnumerable{``0})
```
Converts the value.

|Parameter Name|Remarks|
|--------------|-------|
|value|A @"T:Newtonsoft.Json.Linq.JToken" cast as a @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken".|

_returns: A converted value._

#### Values
```csharp
Newtonsoft.Json.Linq.Extensions.Values(System.Collections.Generic.IEnumerable{Newtonsoft.Json.Linq.JToken})
```
Returns a collection of child values of every object in the source collection.

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the values of every token in the source collection._

#### Values``1
```csharp
Newtonsoft.Json.Linq.Extensions.Values``1(System.Collections.Generic.IEnumerable{Newtonsoft.Json.Linq.JToken})
```
Returns a collection of converted child values of every object in the source collection.

|Parameter Name|Remarks|
|--------------|-------|
|source|An @"T:System.Collections.Generic.IEnumerable`1" of @"T:Newtonsoft.Json.Linq.JToken" that contains the source collection.|

_returns: An @"T:System.Collections.Generic.IEnumerable`1" that contains the converted values of every token in the source collection._


