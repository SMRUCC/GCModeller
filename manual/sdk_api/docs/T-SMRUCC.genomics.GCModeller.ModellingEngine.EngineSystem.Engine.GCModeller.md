---
title: GCModeller
---

# GCModeller
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.html)_





### Methods

#### ConnectDataService
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller.ConnectDataService(System.String)
```
Setup the connection between the data acquisition service to the data storage service.

|Parameter Name|Remarks|
|--------------|-------|
|ServiceURL|-|


#### Load
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller.Load(System.String,Microsoft.VisualBasic.Logging.LogFile,Microsoft.VisualBasic.CommandLine.CommandLine)
```
从数据模型文件之中初始化计算系统

|Parameter Name|Remarks|
|--------------|-------|
|ModelFile|The data model for the target modelling cell system.(所要执行模拟计算的目标细胞的数据模型)|


#### LoadSystemModule
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller.LoadSystemModule(System.String[],Microsoft.VisualBasic.List{System.String})
```
加载系统模块，本操作要先于Initialize操作执行

|Parameter Name|Remarks|
|--------------|-------|
|ModuleAssembly|外部系统模块列表|


#### MemoryDump
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller.MemoryDump(System.String)
```
将计算引擎的工作情况做一次快照

|Parameter Name|Remarks|
|--------------|-------|
|DumpFile|-|



### Properties

#### args
程序最开始输入得到的命令行参数
#### KernelModule
整个系统中的计算核心模块
#### KernelProfile
计算引擎的配置数据
