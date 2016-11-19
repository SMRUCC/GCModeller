# Vector
_namespace: [Microsoft.VisualBasic.Mathematical.BasicR](./index.md)_

@``T:System.Collections.Generic.List`1``



### Methods

#### #ctor
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.#ctor(System.Double,System.Int32)
```
Creates vector with m element and init value specific by init parameter.

|Parameter Name|Remarks|
|--------------|-------|
|init|-|
|m|-|


#### op_Addition
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.op_Addition(System.Double,Microsoft.VisualBasic.Mathematical.BasicR.Vector)
```
实数加向量

|Parameter Name|Remarks|
|--------------|-------|
|a|-|
|v1|-|


#### op_BitwiseOr
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.op_BitwiseOr(Microsoft.VisualBasic.Mathematical.BasicR.Vector,Microsoft.VisualBasic.Mathematical.BasicR.Vector)
```
向量内积

|Parameter Name|Remarks|
|--------------|-------|
|v1|-|
|v2|-|


#### op_Division
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.op_Division(Microsoft.VisualBasic.Mathematical.BasicR.Vector,System.Double)
```
向量 数除，各分量分别除以实数

|Parameter Name|Remarks|
|--------------|-------|
|v1|-|
|a|-|


#### op_ExclusiveOr
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.op_ExclusiveOr(Microsoft.VisualBasic.Mathematical.BasicR.Vector,Microsoft.VisualBasic.Mathematical.BasicR.Vector)
```
向量外积（相当于列向量，乘以横向量）

|Parameter Name|Remarks|
|--------------|-------|
|v1|-|
|v2|-|


#### op_Multiply
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.op_Multiply(System.Double,Microsoft.VisualBasic.Mathematical.BasicR.Vector)
```
向量 数乘

|Parameter Name|Remarks|
|--------------|-------|
|a|-|
|v1|-|


#### op_OnesComplement
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.op_OnesComplement(Microsoft.VisualBasic.Mathematical.BasicR.Vector)
```
向量模的平方

|Parameter Name|Remarks|
|--------------|-------|
|v1|-|


#### op_Subtraction
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.op_Subtraction(System.Double,Microsoft.VisualBasic.Mathematical.BasicR.Vector)
```
实数减向量

|Parameter Name|Remarks|
|--------------|-------|
|a|-|
|v1|-|


#### op_UnaryNegation
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.op_UnaryNegation(Microsoft.VisualBasic.Mathematical.BasicR.Vector)
```
负向量

|Parameter Name|Remarks|
|--------------|-------|
|v1|-|


#### ToString
```csharp
Microsoft.VisualBasic.Mathematical.BasicR.Vector.ToString
```
Display member's data as json array


### Properties

#### Inf
@``F:System.Double.PositiveInfinity``
#### IsNumeric
@``P:Microsoft.VisualBasic.Mathematical.SyntaxAPI.Vectors.GenericVector`1.Dim``为1?即当前的向量对象是否是只包含有一个数字？
#### NAN
@``F:System.Double.NaN``
#### Zero
Only one number in the vector and its value is ZERO
