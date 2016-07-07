---
title: KernelDriver`3
---

# KernelDriver`3
_namespace: [SMRUCC.genomics.GCModeller.Framework.Kernel_Driver](N-SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.html)_

Driver of the GCModeller system kernel.(计算引擎核心的驱动程序)



### Methods

#### __invokeDataAcquisition
```csharp
SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.KernelDriver`3.__invokeDataAcquisition
```
数据采集程序的驱动句柄

#### LoadEngineKernel
```csharp
SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.KernelDriver`3.LoadEngineKernel(`2)
```
Load the simulation kernel into the calculation kernel driver and then initialize a data adapter for the kernel.
 (加载计算内核，并且为该内核初始化一个数据采集适配器对象.)

|Parameter Name|Remarks|
|--------------|-------|
|kernel|-|


#### Run
```csharp
SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.KernelDriver`3.Run
```
The engine kernel driver running the loadded kernel object.(内核驱动程序运行已经加载的内核程序)


### Properties

#### _runtimeTicks
The current ticks since from the start of running.
 (从运行开始后到当前的时间中所流逝的内核循环次数)
