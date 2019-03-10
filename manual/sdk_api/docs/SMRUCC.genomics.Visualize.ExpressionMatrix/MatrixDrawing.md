# MatrixDrawing
_namespace: [SMRUCC.genomics.Visualize.ExpressionMatrix](./index.md)_





### Methods

#### CreateAlphabetTagSerials
```csharp
SMRUCC.genomics.Visualize.ExpressionMatrix.MatrixDrawing.CreateAlphabetTagSerials(System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|dat|数据必须是已经去除掉了重复的|


#### NormalMatrix
```csharp
SMRUCC.genomics.Visualize.ExpressionMatrix.MatrixDrawing.NormalMatrix(Microsoft.VisualBasic.Data.csv.DocumentStream.File)
```
tag1 tag2 tag3 tag4 ...
 tag1 ... ... ... ... ... 
 tag2 ... ... ... ... ...
 tag3 ... ... ... ... ...
 tag4 ... ... ... ... ...
 .... ... ... ... ... ...

|Parameter Name|Remarks|
|--------------|-------|
|MAT|-|


#### NormalMatrixTriangular
```csharp
SMRUCC.genomics.Visualize.ExpressionMatrix.MatrixDrawing.NormalMatrixTriangular(Microsoft.VisualBasic.Data.csv.DocumentStream.File)
```
绘制上三角形
 
 tag1 tag2 tag3 tag4 ...
 ... ... ... ... ... tag1
 ... ... ... ... tag2
 ... ... ... tag3
 ... ... tag4
 ... tag5

|Parameter Name|Remarks|
|--------------|-------|
|MAT|-|



