---
title: MetabolismCompartment
---

# MetabolismCompartment
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.html)_

代谢反应所构成的网络系统对象，同时也是一个Compartment对象



### Methods

#### get_DataSerializerHandles
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment.get_DataSerializerHandles
```
返回代谢物UniqueId列表，对于反应过程的数据则定义于DelegateSystem处

#### InitalizeTrimedHandles
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.MetabolismCompartment.InitalizeTrimedHandles
```
本方法是在初始化完毕基因表达调控系统之后进行的，因为代谢组是第一个被初始化的系统，而此时基因表达调控网络系统还是Null空值，所以本方法需要在调控网络初始化完毕之后由外部的其他对象所调用


### Properties

#### DataSource
返回代谢物的浓度
#### ProteinCPLXAssemblies
蛋白质复合物形成的容器：信号转导网络
#### TransmembraneFluxs
这个不进行计算，仅作为数据观察的接口而是用，对于本部分的反应的计算的驱动，则来自于培养基对象之中
#### TrimMetabolism
是否从代谢组的数据中去除RNA和蛋白质的浓度
