---
title: Kernel
---

# Kernel
_namespace: [SMRUCC.genomics.Analysis.SSystem.Kernel](N-SMRUCC.genomics.Analysis.SSystem.Kernel.html)_

The simulation system kernel.



### Methods

#### __innerTicks
```csharp
SMRUCC.genomics.Analysis.SSystem.Kernel.Kernel.__innerTicks(System.Int32)
```
The kernel loop.(内核循环, 会在这里更新数学表达式计算引擎的环境变量)

#### Run
```csharp
SMRUCC.genomics.Analysis.SSystem.Kernel.Kernel.Run(SMRUCC.genomics.Analysis.SSystem.Script.Model,System.Double)
```
Run a compiled model.(运行一个已经编译好的模型文件)

|Parameter Name|Remarks|
|--------------|-------|
|Model|-|



### Properties

#### __engine
模拟器的数学计算引擎
#### Channels
Alter the system state.
#### dataSvr
Data collecting
#### Kicks
Object that action the disturbing
#### RuntimeTicks
Gets the system run time ticks
#### Vars
Store the system state.
