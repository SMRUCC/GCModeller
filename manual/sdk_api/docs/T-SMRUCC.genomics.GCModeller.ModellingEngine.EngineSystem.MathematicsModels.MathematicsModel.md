---
title: MathematicsModel
---

# MathematicsModel
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.html)_





### Methods

#### A
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.MathematicsModel.A(System.Double,System.Double,System.Double)
```
在初始化带些方程的时候计算A系数所需要的

|Parameter Name|Remarks|
|--------------|-------|
|K|-|
|T|-|
|Ea|右端分子质量与左端分子质量之差|


#### Arrhenius
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.MathematicsModel.Arrhenius(System.Double,System.Double,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|A|-|
|Ea|右端分子质量与左端分子质量之差|
|T|当前温度|


#### Hill
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.MathematicsModel.Hill(System.Double,System.Double,System.Double,System.Double,System.Double,System.Double)
```
希尔函数

|Parameter Name|Remarks|
|--------------|-------|
|k1n|k1-|
|k1p|k1+|
|k2|k2小于k3，正调控|
|k3|-|
|X|调控因子的浓度|
|n|需要多少个调控因子参加调控反应|


#### MichaelisMenten
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.MathematicsModel.MichaelisMenten(System.Double,System.Double,System.Double)
```


|Parameter Name|Remarks|
|--------------|-------|
|vMax|最大反应速率|
|S|底物的浓度|
|Km|米氏常数|


#### NormalDistribution
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.MathematicsModels.MathematicsModel.NormalDistribution(System.Double,System.Double,System.Double)
```
正态分布函数，采用默认参数则为标准正太分布

|Parameter Name|Remarks|
|--------------|-------|
|x|-|
|u|-|
|sigma|-|



### Properties

#### R
摩尔气体常数
