---
title: CultivationMediums
---

# CultivationMediums
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.html)_





### Methods

#### __innerTicks
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CultivationMediums.__innerTicks(System.Int32)
```
假设培养基中的物质很多，所以浓度基本不会被改变，故而在这里每一次循环都会将培养基之中指定的成分的浓度设置为原来的水平


### Properties

#### _IsLiquidBrothType
是否为液体培养基，是的话，则虚拟细胞之中的水充足，不会被改变
#### _LevelResetList
@"M:SMRUCC.genomics.GCModeller.Framework.Kernel_Driver.ReactorMachine`2.TICK"过程之中会被重新设置水平的代谢物，这个列表和@"F:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.ObjectModels.SubSystem.CultivationMediums._CultivationMediumsModel"[培养基的数据模型]之中的顺序是一致的
#### CultivationMediums
用于表示培养基的生化反应网络的对象类型
#### Temperature
外部程序是通过本属性来改变整个体系的环境温度的
