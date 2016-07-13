---
title: List`1
---

# List`1
_namespace: [Microsoft.VisualBasic](N-Microsoft.VisualBasic.html)_

Represents a strongly typed list of objects that can be accessed by index. Provides
 methods to search, sort, and manipulate lists.To browse the .NET Framework source
 code for this type, see the Reference Source.



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.List`1.#ctor(System.Int32)
```
Initializes a new instance of the List`1 class that
 is empty and has the specified initial capacity.

|Parameter Name|Remarks|
|--------------|-------|
|capacity|The number of elements that the new list can initially store.|


#### op_Addition
```csharp
Microsoft.VisualBasic.List`1.op_Addition(System.Collections.Generic.IEnumerable{`0},Microsoft.VisualBasic.List{`0})
```
Adds the elements of the specified collection to the end of the System.Collections.Generic.List`1.

|Parameter Name|Remarks|
|--------------|-------|
|vals|-|
|list|-|


#### op_Exponent
```csharp
Microsoft.VisualBasic.List`1.op_Exponent(Microsoft.VisualBasic.List{`0},System.Func{`0,System.Boolean})
```
Find a item in the list

|Parameter Name|Remarks|
|--------------|-------|
|list|-|
|find|-|


#### op_GreaterThan
```csharp
Microsoft.VisualBasic.List`1.op_GreaterThan(Microsoft.VisualBasic.List{`0},System.String)
```
Dump this collection data to the file system.

|Parameter Name|Remarks|
|--------------|-------|
|source|-|
|path|-|


#### op_Subtraction
```csharp
Microsoft.VisualBasic.List`1.op_Subtraction(Microsoft.VisualBasic.List{`0},`0)
```
Removes the first occurrence of a specific object from the List`1.

|Parameter Name|Remarks|
|--------------|-------|
|list|-|
|x|The object to remove from the List`1. The value can
 be null for reference types.|


#### op_UnaryPlus
```csharp
Microsoft.VisualBasic.List`1.op_UnaryPlus(Microsoft.VisualBasic.List{`0})
```
Move Next

|Parameter Name|Remarks|
|--------------|-------|
|list|-|


#### PopAll
```csharp
Microsoft.VisualBasic.List`1.PopAll
```
Pop all of the elements value in to array from the list object and then clear all of the list data.


