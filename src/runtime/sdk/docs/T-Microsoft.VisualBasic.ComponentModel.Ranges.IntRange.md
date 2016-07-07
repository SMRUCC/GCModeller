---
title: IntRange
---

# IntRange
_namespace: [Microsoft.VisualBasic.ComponentModel.Ranges](N-Microsoft.VisualBasic.ComponentModel.Ranges.html)_

Represents an integer range with minimum and maximum values



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.ComponentModel.Ranges.IntRange.#ctor(System.Int32,System.Int32)
```
Initializes a new instance of the @"T:Microsoft.VisualBasic.ComponentModel.Ranges.IntRange" class

|Parameter Name|Remarks|
|--------------|-------|
|min|Minimum value of the range|
|max|Maximum value of the range|


#### IsInside
```csharp
Microsoft.VisualBasic.ComponentModel.Ranges.IntRange.IsInside(Microsoft.VisualBasic.ComponentModel.Ranges.IntRange)
```
Check if the specified range is inside this range

|Parameter Name|Remarks|
|--------------|-------|
|range|Range to check|

_returns: True if the specified range is inside this range or
 false otherwise._

#### IsOverlapping
```csharp
Microsoft.VisualBasic.ComponentModel.Ranges.IntRange.IsOverlapping(Microsoft.VisualBasic.ComponentModel.Ranges.IntRange)
```
Check if the specified range overlaps with this range

|Parameter Name|Remarks|
|--------------|-------|
|range|Range to check for overlapping|

_returns: True if the specified range overlaps with this range or
 false otherwise._


### Properties

#### Length
Length of the range (deffirence between maximum and minimum values)
#### Max
Maximum value
#### Min
Minimum value
