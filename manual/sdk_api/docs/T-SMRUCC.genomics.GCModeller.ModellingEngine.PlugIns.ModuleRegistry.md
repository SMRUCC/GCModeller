---
title: ModuleRegistry
---

# ModuleRegistry
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns](N-SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.html)_

The registry object for the externel system module assembly.(系统外部模块的注册表对象)



### Methods

#### GetModule
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.GetModule(System.String)
```
获取一个已经在系统内注册的外部模块的文件路径

|Parameter Name|Remarks|
|--------------|-------|
|ModuleName|目标外部模块的模块名称或者文件路径|


#### Load
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.Load(System.String)
```
加载一个注册表文件

|Parameter Name|Remarks|
|--------------|-------|
|File|-|

_returns: 返回读取得到的一个注册表文件_

#### LoadModule
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.LoadModule(System.String)
```
分析一个外部的系统模块编译文档文件

|Parameter Name|Remarks|
|--------------|-------|
|AssemblyPath|目标外部模块的文件路径|


#### Registry
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.PlugIns.ModuleRegistry.Registry(System.String)
```
注册一个外部的系统模块

|Parameter Name|Remarks|
|--------------|-------|
|AssemblyPath|目标系统外部模块文件的文件路径|



### Properties

#### Modules
这个对象记录着在当前模块注册表文件之中所注册的外部系统模块
