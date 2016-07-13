---
title: VBMathExtensions
---

# VBMathExtensions
_namespace: [Microsoft.VisualBasic](N-Microsoft.VisualBasic.html)_





### Methods

#### EuclideanDistance
```csharp
Microsoft.VisualBasic.VBMathExtensions.EuclideanDistance(System.Double[],System.Double[])
```


|Parameter Name|Remarks|
|--------------|-------|
|a|Point A|
|b|Point B|


#### Hypot
```csharp
Microsoft.VisualBasic.VBMathExtensions.Hypot(System.Double,System.Double)
```
sqrt(a^2 + b^2) without under/overflow.

|Parameter Name|Remarks|
|--------------|-------|
|a|-|
|b|-|


#### IsPowerOf2
```csharp
Microsoft.VisualBasic.VBMathExtensions.IsPowerOf2(System.Int32)
```
Checks if the specified integer is power of 2.

|Parameter Name|Remarks|
|--------------|-------|
|x|Integer number to check.|

_returns: Returns true if the specified number is power of 2.
 Otherwise returns false._

#### Log2
```csharp
Microsoft.VisualBasic.VBMathExtensions.Log2(System.Int32)
```
Get base of binary logarithm.

|Parameter Name|Remarks|
|--------------|-------|
|x|Source integer number.|

_returns: Power of the number (base of binary logarithm)._

#### Max
```csharp
Microsoft.VisualBasic.VBMathExtensions.Max(System.Int32,System.Int32,System.Int32)
```
return the maximum of a, b and c

|Parameter Name|Remarks|
|--------------|-------|
|a|-|
|b|-|
|c|
 @return |


#### PI
```csharp
Microsoft.VisualBasic.VBMathExtensions.PI(System.Collections.Generic.IEnumerable{System.Double})
```
Continues multiply operations.(连续乘法)

|Parameter Name|Remarks|
|--------------|-------|
|data|-|


#### PoissonPDF
```csharp
Microsoft.VisualBasic.VBMathExtensions.PoissonPDF(System.Int32,System.Double)
```
Returns the PDF value at x for the specified Poisson distribution.

#### Pow2
```csharp
Microsoft.VisualBasic.VBMathExtensions.Pow2(System.Int32)
```
Calculates power of 2.

|Parameter Name|Remarks|
|--------------|-------|
|power|Power to raise in.|

_returns: Returns specified power of 2 in the case if power is in the range of
 [0, 30]. Otherwise returns 0._

#### RMS
```csharp
Microsoft.VisualBasic.VBMathExtensions.RMS(System.Collections.Generic.IEnumerable{System.Double})
```
Root mean square.(均方根)

#### STD
```csharp
Microsoft.VisualBasic.VBMathExtensions.STD(System.Collections.Generic.IEnumerable{System.Single})
```
Standard Deviation

#### Sum
```csharp
Microsoft.VisualBasic.VBMathExtensions.Sum(System.Collections.Generic.IEnumerable{System.Boolean})
```
Logical true values are regarded as one, false values as zero. For historical reasons, NULL is accepted and treated as if it were integer(0).

|Parameter Name|Remarks|
|--------------|-------|
|bc|-|



