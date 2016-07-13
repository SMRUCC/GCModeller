---
title: FBAlpRSolver
---

# FBAlpRSolver
_namespace: [SMRUCC.genomics.Analysis.FBA_DP](N-SMRUCC.genomics.Analysis.FBA_DP.html)_

求解FBA线性规划问题的模块对象



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Analysis.FBA_DP.FBAlpRSolver.#ctor(System.String)
```
初始化一个求解FBA线性规划问题的模块对象实例

|Parameter Name|Remarks|
|--------------|-------|
|rBin|Parameter for manual initialize the REngine, Example likes: "C:\Program Files\R\R-3.1.0\bin".|


#### RSolving
```csharp
SMRUCC.genomics.Analysis.FBA_DP.FBAlpRSolver.RSolving(SMRUCC.genomics.Analysis.FBA_DP.lpSolveRModel,System.String@)
```
using the R program to solve the linear programming problem that from the fba model.
 (使用R模块来求解FBA模型中的线性规划问题, {ObjectiveFunction, FluxDistribution()})

|Parameter Name|Remarks|
|--------------|-------|
|lpSolveRModel|-|
|script|Generated script output|

_returns: {ObjectiveFunction, FluxDistribution()}_


