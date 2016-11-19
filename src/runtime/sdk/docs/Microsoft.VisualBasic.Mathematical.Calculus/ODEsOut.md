# ODEsOut
_namespace: [Microsoft.VisualBasic.Mathematical.Calculus](./index.md)_

ODEs output



### Methods

#### DataFrame
```csharp
Microsoft.VisualBasic.Mathematical.Calculus.ODEsOut.DataFrame(System.String,System.Int32)
```
Generates datafram and then can makes the result save data into a csv file.

|Parameter Name|Remarks|
|--------------|-------|
|xDisp|-|
|fix%|Formats output by using @``M:System.Math.Round(System.Decimal)``|


#### GetY0
```csharp
Microsoft.VisualBasic.Mathematical.Calculus.ODEsOut.GetY0
```
Using the first value of @``P:Microsoft.VisualBasic.Mathematical.Calculus.ODEsOut.y`` as ``y0``

#### Join
```csharp
Microsoft.VisualBasic.Mathematical.Calculus.ODEsOut.Join
```
Merge @``P:Microsoft.VisualBasic.Mathematical.Calculus.ODEsOut.y0`` into @``P:Microsoft.VisualBasic.Mathematical.Calculus.ODEsOut.params``

#### LoadFromDataFrame
```csharp
Microsoft.VisualBasic.Mathematical.Calculus.ODEsOut.LoadFromDataFrame(System.String,System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|csv$|-|
|noVars|ODEs Parameter value is not exists in the data file?|



### Properties

#### HaveNaN
Is there NAN value in the function value @``P:Microsoft.VisualBasic.Mathematical.Calculus.ODEsOut.y`` ???
