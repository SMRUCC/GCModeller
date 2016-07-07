---
title: I_SystemEventDriver
---

# I_SystemEventDriver
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.html)_

Driver for all of the events in the target @"F:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.I_SystemEventDriver._cellObject"[virtual cell.]
 (目标虚拟细胞对象之中的所有网络之中的边的驱动程序模块)



### Methods

#### EmptyEvent
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.I_SystemEventDriver.EmptyEvent
```
DO_NOTHING

#### InvokeEvents
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.I_SystemEventDriver.InvokeEvents
```
Running the virtual cell from here!

#### JoinEvents
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.I_SystemEventDriver.JoinEvents(System.Collections.Generic.IEnumerable{SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.Module.FluxObject})
```
Adding the flux object events into this @"T:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.I_SystemEventDriver"

|Parameter Name|Remarks|
|--------------|-------|
|Events|-|



### Properties

#### _innerAttachedEventHandle
@"M:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.I_SystemEventDriver.___Internal_get_attachedEvent"函数的执行入口点
#### _lstAttachedEvent
附加需要处理的额外事件
