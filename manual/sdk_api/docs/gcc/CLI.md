# CLI
_namespace: [gcc](./index.md)_





### Methods

#### AddRule
```csharp
gcc.CLI.AddRule(Microsoft.VisualBasic.CommandLine.CommandLine)
```
向一个编译好的计算机模型文件之中添加相互作用规则

|Parameter Name|Remarks|
|--------------|-------|
|CommandLine|-|

> 
>  由于规则是按照结构域来标识的，故而，首先需要进行结构域的分析
>  之后将结构域拓展为具体的目标蛋白质
>  最后将拓展的新反应规则添加进入计算机模型之中
>  

#### CompileMetaCyc
```csharp
gcc.CLI.CompileMetaCyc(Microsoft.VisualBasic.CommandLine.CommandLine)
```
Compile a metacyc database into a gcml model file.(将一个MetaCyc数据库编译为gcml计算机模型文件)

|Parameter Name|Remarks|
|--------------|-------|
|CommandLine|-|



