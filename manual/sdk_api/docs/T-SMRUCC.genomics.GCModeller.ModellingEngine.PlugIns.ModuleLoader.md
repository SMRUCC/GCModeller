---
title: ModuleLoader
---

# ModuleLoader
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns](N-SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.html)_

External model loading operator.(外部计算模型加载对象，在加载外部计算模块的同时，还会向计算框架中的数据采集服务处理队列之中加载构造出来的数据采集服务)

> 先从外部加载对象模型，当加载计算模型之后再加载数据模型


### Methods

#### #ctor
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleLoader.#ctor(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller)
```


|Parameter Name|Remarks|
|--------------|-------|
|Kernel|目标计算引擎框架类型实例|


#### Load_InternalSystemModule
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleLoader.Load_InternalSystemModule(Microsoft.VisualBasic.List{SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ISystemFrameworkEntry.Types})
```
计算核心加载系统内部的计算模块

|Parameter Name|Remarks|
|--------------|-------|
|ModuleNotLoaded|尚未加载的系统模块的列表|


#### LoadInternalMetabolismModule
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleLoader.LoadInternalMetabolismModule(SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CellSystem,SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller)
```
加载内部的代谢组计算模块，并初始化代谢组计算模块的数据采集服务

|Parameter Name|Remarks|
|--------------|-------|
|KernelModule|-|
|EngineSystem|-|


#### LoadModel
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleLoader.LoadModel(System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|AssemblyPath|-|

_returns: 返回所加载的外部系统模块的模块类型，当加载不成功的时候返回-1_

#### LoadModules
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleLoader.LoadModules(System.String[])
```
外部系统模块的文件路径

|Parameter Name|Remarks|
|--------------|-------|
|Systems|-|



### Properties

#### DisabledModules
下列类型的模块将不会被加载
