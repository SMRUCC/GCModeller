---
title: lpSolveRModel
---

# lpSolveRModel
_namespace: [SMRUCC.genomics.Analysis.FBA_DP](N-SMRUCC.genomics.Analysis.FBA_DP.html)_

Class object using for generate the R script for the R package lpSolveAPI.(用于生成lpSolverAPI所需的脚本的类型对象)



### Methods

#### __add_constraint
```csharp
SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel.__add_constraint(System.Double[],System.String,System.Double)
```
Generate a S matrix line.(生成用于表示FBA模型中的S矩阵中的一行的R脚本)

|Parameter Name|Remarks|
|--------------|-------|
|vectorData|-|
|direction|-|
|constraintValue|-|

_returns: add.constraint(lprec, c(0.24, 0, 11.31, 0), "=", 14.8)_

#### __getLowerbound
```csharp
SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel.__getLowerbound
```
获得每一个反应活动的流量下线

#### __getUpbound
```csharp
SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel.__getUpbound
```
获得每一个反应活动的流量上限

#### __set_objfn
```csharp
SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel.__set_objfn(System.Double[],System.String)
```
Generate objective function for FBA model in lpSolveAPI.

|Parameter Name|Remarks|
|--------------|-------|
|vectorData|-|


#### CreateResultFile
```csharp
SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel.CreateResultFile(System.Collections.Generic.KeyValuePair{System.String,System.String[]})
```
将LpSolveAPI返回的结果生成数据文件以进行导出

|Parameter Name|Remarks|
|--------------|-------|
|resultData|{ObjectiveFunctionValue, FluxDistributions}|


#### getConstraint
```csharp
SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel.getConstraint(System.String)
```
{direction, constraint}

|Parameter Name|Remarks|
|--------------|-------|
|metabolite|-|


#### getMatrix
```csharp
SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel.getMatrix
```
横行为代谢物，列为代谢过程，元素为化学计量数

#### SetObjectiveFunc
```csharp
SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel.SetObjectiveFunc(System.String[])
```


|Parameter Name|Remarks|
|--------------|-------|
|factors|可以是基因号或者反应的编号，基因号需要在这个函数里面进行映射转换|



### Properties

#### __fluxObjective
@"M:SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel.SetObjectiveFunc(System.String[])" 转换映射得到的一个反应过程的集合
#### _constraint
changes = 0; 
 对于FBA模型而言，每一种代谢物的浓度都是被假设为稳定状态，即在平衡态之中没有浓度变化
#### CDirect
Direction of Objective Function.(目标函数的约束方向)
#### fluxColumns
得到每一个反应过程的标识符
