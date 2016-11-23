# SBML
_namespace: [SMRUCC.genomics.Analysis.SSystem.Script](./index.md)_

SBML模型编译器



### Methods

#### __generateSystem
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.SBML.__generateSystem(SMRUCC.genomics.Analysis.SSystem.Script.Model)
```
Generate the equation group of the target modelling system.(生成目标模型系统的方程组)

|Parameter Name|Remarks|
|--------------|-------|
|Model|-|


#### __strip
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.SBML.__strip
```
需要在这里将``-``连接符替换为下划线``_``不然在解析数学表达式的时候会被当作为减号

#### __where
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.SBML.__where(SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.var,SMRUCC.genomics.Analysis.SSystem.Script.Model)
```
检查目标反应物对象是否存在于模型的表达式之中

|Parameter Name|Remarks|
|--------------|-------|
|x|-|
|model|-|


#### GenerateFunction
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.SBML.GenerateFunction(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Metabolite|-|

> 
>  + 在计算消耗的部分的时候，S系统方程中当前的底物是作为反应物而被消耗掉的
>  + 在计算生成的部分的时候，S系统方程之中是消耗来源的反应物而生成的当前底物
>  
>  故而两个方向都是取值来源为反应物
>  


