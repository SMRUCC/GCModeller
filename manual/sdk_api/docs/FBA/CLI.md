# CLI
_namespace: [FBA](./index.md)_





### Methods

#### __getObjectives
```csharp
FBA.CLI.__getObjectives(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|file|-|
|type|lst/pathway/module|


#### AnalysisPhenotype
```csharp
FBA.CLI.AnalysisPhenotype(Microsoft.VisualBasic.CommandLine.CommandLine)
```
计算出突变对表型的相关度，应该计算一次野生型和突变型的数据

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### CompileMetaCyc
```csharp
FBA.CLI.CompileMetaCyc(Microsoft.VisualBasic.CommandLine.CommandLine)
```
metacyc --> FBA/FBA2

|Parameter Name|Remarks|
|--------------|-------|
|CommandLine|-|


#### CompileSBML
```csharp
FBA.CLI.CompileSBML(Microsoft.VisualBasic.CommandLine.CommandLine)
```
SBML --> FBA

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### FluxCoefficient
```csharp
FBA.CLI.FluxCoefficient(Microsoft.VisualBasic.CommandLine.CommandLine)
```
计算调控因子和代谢过程相关性，从单个计算结果之中进行分析

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### FuncCoefficient
```csharp
FBA.CLI.FuncCoefficient(Microsoft.VisualBasic.CommandLine.CommandLine)
```
目标函数和基因之间的相关性，由于这里的目标函数的结果是从批量计算的结果之中所导出合并的一个矩阵，故而输入的基因表达量和实验用的sampleTable都是一样的，
 所以这里的/in参数的作用是得到计算相关性的基因的表达的数据，用哪一个文件夹的数据都无所谓

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### ImportsRxns
```csharp
FBA.CLI.ImportsRxns(Microsoft.VisualBasic.CommandLine.CommandLine)
```
向数据库之中导入没有的反应过程的数据记录

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### ObjMAT
```csharp
FBA.CLI.ObjMAT(Microsoft.VisualBasic.CommandLine.CommandLine)
```
将Object Function的结果合并为一个矩阵

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### PhenoOUT_MAT
```csharp
FBA.CLI.PhenoOUT_MAT(Microsoft.VisualBasic.CommandLine.CommandLine)
```
将Object Function的结果合并为一个矩阵

|Parameter Name|Remarks|
|--------------|-------|
|args|假设所有的实验都是相同的数据的基础上面做出来的，唯一的区别是目标函数是不同的表型|


#### PhenotypeAnalysisBatch
```csharp
FBA.CLI.PhenotypeAnalysisBatch(Microsoft.VisualBasic.CommandLine.CommandLine)
```
Batch task schedule for @``M:FBA.CLI.rFBABatch(Microsoft.VisualBasic.CommandLine.CommandLine)``

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### rFBABatch
```csharp
FBA.CLI.rFBABatch(Microsoft.VisualBasic.CommandLine.CommandLine)
```
对一个指定的性状计算出sampleTable里面的所有的sample条件下的变化

|Parameter Name|Remarks|
|--------------|-------|
|args|-|


#### Solve
```csharp
FBA.CLI.Solve(Microsoft.VisualBasic.CommandLine.CommandLine)
```
Solve a metabolism network model using the FBA method, the model data was comes from a sbml model or compiled gcml model.
 (使用FBA方法对一个代谢网络问题进行求解，模型数据来自于一个SBML文件或者一个已经编译好的模型文件)

|Parameter Name|Remarks|
|--------------|-------|
|args|-|



### Properties

#### CompileMethods
The compiler support the metacyc database and sbml model file format currently.(编译器当前仅支持MetaCyc数据库和SBML标准模型数据源)
