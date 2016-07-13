---
title: S4Object
---

# S4Object
_namespace: [RDotNET](N-RDotNET.html)_

An S4 object



### Methods

#### #ctor
```csharp
RDotNET.S4Object.#ctor(RDotNET.REngine,System.IntPtr)
```
Create a new S4 object

|Parameter Name|Remarks|
|--------------|-------|
|engine__1|R engine|
|pointer|pointer to native S4 SEXP|


#### GetClassDefinition
```csharp
RDotNET.S4Object.GetClassDefinition
```
Gets the class representation.
_returns: The class representation of the S4 class._

#### GetSlotTypes
```csharp
RDotNET.S4Object.GetSlotTypes
```
Gets slot names and types.
_returns: Slot names._

#### HasSlot
```csharp
RDotNET.S4Object.HasSlot(System.String)
```
Is a slot name valid.

|Parameter Name|Remarks|
|--------------|-------|
|slotName|the name of the slot|

_returns: whether a slot name is present in the object_


### Properties

#### dotSlotNamesFunc
Function .slotNames
#### Item
Gets/sets the value of a slot
#### SlotCount
Gets the number of slot names
#### SlotNames
Gets the slot names for this object. The values are cached once retrieved the first time. 
 Note this is equivalent to the function '.slotNames' in R, not 'slotNames'
