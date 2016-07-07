---
title: ColorRender
---

# ColorRender
_namespace: [SMRUCC.genomics.Visualize.GeneticClock](N-SMRUCC.genomics.Visualize.GeneticClock.html)_

获取值大小梯度颜色



### Methods

#### #ctor
```csharp
SMRUCC.genomics.Visualize.GeneticClock.ColorRender.#ctor(SMRUCC.genomics.InteractionModel.DataServicesExtension.SerialsData[],System.Boolean)
```


|Parameter Name|Remarks|
|--------------|-------|
|DataSet|-|
|Global|颜色的梯度计算是否为全局性的，默认为全局性的|


#### GetColorRenderingProfiles
```csharp
SMRUCC.genomics.Visualize.GeneticClock.ColorRender.GetColorRenderingProfiles
```
获取和数值大小相关的密度颜色

#### GetDesityRule
```csharp
SMRUCC.genomics.Visualize.GeneticClock.ColorRender.GetDesityRule(System.Int32)
```


|Parameter Name|Remarks|
|--------------|-------|
|counts|-|


#### InternalGlobalCalculation
```csharp
SMRUCC.genomics.Visualize.GeneticClock.ColorRender.InternalGlobalCalculation(SMRUCC.genomics.InteractionModel.DataServicesExtension.SerialsData[])
```
全局的值就只有一个

|Parameter Name|Remarks|
|--------------|-------|
|DataSet|-|


#### InternalLocalCalculation
```csharp
SMRUCC.genomics.Visualize.GeneticClock.ColorRender.InternalLocalCalculation(SMRUCC.genomics.InteractionModel.DataServicesExtension.SerialsData[])
```
单条的每一个样品数据都有自己的映射

|Parameter Name|Remarks|
|--------------|-------|
|DataSet|-|


#### Interpolate
```csharp
SMRUCC.genomics.Visualize.GeneticClock.ColorRender.Interpolate(System.Double[])
```
对数据进行平均插值

|Parameter Name|Remarks|
|--------------|-------|
|data|-|



