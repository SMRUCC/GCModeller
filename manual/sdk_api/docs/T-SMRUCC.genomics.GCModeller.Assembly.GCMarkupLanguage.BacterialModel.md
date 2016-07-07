---
title: BacterialModel
---

# BacterialModel
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.html)_

LansSystemsBiologyModel, this model file contains all of the required data for the GCModeller virtual cell simulation.
 (细菌细胞的计算模型文件，在本模型文件之中包含了所有的GCModeller进行模拟计算所需要的信息)

> 
>  一次基本的计算包括下面的步骤：
>  1. FBA模型求解代谢物的浓度分布
>  2. 根据代谢物的浓度分布计算出每一个基因的表达水平（转录水平）
>  3. 根据调控关系计算出每一种酶分子和调控因子的浓度分布
>  4. 下一个迭代循环直到到达最终的结束时间(RUN 命令行)
>  


### Methods

#### Load
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel.Load(System.String)
```
Read the compiled model file that in the xml format.(读取已经编译好的保存为XML格式的模型文件)

|Parameter Name|Remarks|
|--------------|-------|
|FilePath|Xml文件格式的计算机模型文件的文件路径|


#### Visualizing
```csharp
SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.BacterialModel.Visualizing(System.String,System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Input|The input compiled xml model.|
|Output|The output html file.|
|Style|The xldt style file.|



### Properties

#### DispositionModels
仅包含有两个元素：多肽链分子和RNA分子的降解反应
#### Height
n reactions
#### ProteinAssemblies
形成蛋白质复合物的规则：信号转导网络
#### SystemVariables
一些关键的系统保留变量
#### Width
m metabolites
