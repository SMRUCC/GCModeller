---
title: IArrayPool`1
---

# IArrayPool`1
_namespace: [Newtonsoft.Json](N-Newtonsoft.Json.html)_

Provides an interface for using pooled arrays.



### Methods

#### Rent
```csharp
Newtonsoft.Json.IArrayPool`1.Rent(System.Int32)
```
Rent a array from the pool. This array must be returned when it is no longer needed.

|Parameter Name|Remarks|
|--------------|-------|
|minimumLength|The minimum required length of the array. The returned array may be longer.|

_returns: The rented array from the pool. This array must be returned when it is no longer needed._

#### Return
```csharp
Newtonsoft.Json.IArrayPool`1.Return(`0[])
```
Return an array to the pool.

|Parameter Name|Remarks|
|--------------|-------|
|array|The array that is being returned.|



