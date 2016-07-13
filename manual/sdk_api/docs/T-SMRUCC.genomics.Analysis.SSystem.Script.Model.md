---
title: Model
---

# Model
_namespace: [SMRUCC.genomics.Analysis.SSystem.Script](N-SMRUCC.genomics.Analysis.SSystem.Script.html)_

可以被保存至文件的脚本模型对象



### Methods

#### Load
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.Model.Load(System.String)
```
Load a model from a compiled xml model file.
 (从一个已经编译好的XML文件加载)

|Parameter Name|Remarks|
|--------------|-------|
|Path|The target compiled xml model file.(目标已经编译好的XML模型文件)|


#### op_Implicit
```csharp
SMRUCC.genomics.Analysis.SSystem.Script.Model.op_Implicit(System.String)~SMRUCC.genomics.Analysis.SSystem.Script.Model
```
Load from a script file.
 (从一个脚本源文件中获取模型数据)

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|



### Properties

#### Experiments
The disturbing factors in this system.
 (系统中的干扰因素的集合)
#### FinalTime
The ticks count value of the time that exit this simulation.
 (整个内核运行的退出时间)
#### sEquations
The data channel in this system kernel.
 (系统中的反应过程数据通道)
#### UserFunc
The user define function.
#### Vars
A collection of the system variables.
 (系统中的运行变量的集合)
