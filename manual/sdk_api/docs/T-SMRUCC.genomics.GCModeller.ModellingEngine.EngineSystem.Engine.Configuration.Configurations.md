---
title: Configurations
---

# Configurations
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration](N-SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration.html)_

You should using method @"M:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration.Configurations.Load(System.String,System.Boolean)" for data loading, because this is from the requirement of the property @"P:Microsoft.VisualBasic.ComponentModel.ITextFile.FilePath".
 (对于本对象类型，务必要使用@"M:SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration.Configurations.Load(System.String,System.Boolean)"方法进行加载，因为@"P:Microsoft.VisualBasic.ComponentModel.ITextFile.FilePath"属性需要在后面记载数据的时候被使用到)



### Methods

#### Load
```csharp
SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.Configuration.Configurations.Load(System.String,System.Boolean)
```
假若目标文件不存在，则会返回一个默认的文件数据，假若不希望返回默认数据，请将参数**returnDefaul**设置为False.

|Parameter Name|Remarks|
|--------------|-------|
|Path|-|
|returnDefaul|当目标文件**Path**不存在的时候，是否返回默认的配置数据，默认值为返回该默认数据|



### Properties

#### CommitLoopsInterval
仅针对MYSQL数据存储服务有效的一个配置参数，用于指示计算引擎想数据库服务器提交数据的时间间隔
#### DataStorageUrl

#### ExperimentData
Csv data file file path.(CSV格式的实验数据文件的文件路径)
#### GeneMutations
基因突变数据，可以为一个列表或者指向一个Csv文件的文件路径
#### KernelCycles
The total simulation time, kernel cycles.(内核循环的次数，即本属性值表示总的模拟计算的时间)
