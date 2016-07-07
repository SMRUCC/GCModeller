---
title: Regulator
---

# Regulator
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.html)_

对基因表达调控起作用的对象分子，在这里是一个调控因子对一个调控对象




### Properties

#### EnzymeActivityRegulationTypes
【Regulation: protein activity regulation】
 【Regulation-of-Enzyme-Activity: enzymatic reaction regulation】
 【Regulation-of-Reactions: reaction regulation】
#### ProteinAssembly
该调控因子的@"P:SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.ComponentModels.T_MetaCycEntity`1.Identifier"[蛋白质复合物的组成]
#### Regulates
调控的目标对象
#### Weight
本调控因子对目标调控对象的权重(可以看作为皮尔森相关系数)，对于转录调控因子而言，Key属性指的是目标motif所处的TranscriptUnit的编号，
 与@"P:SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator.Regulates"所不同的是，@"P:SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.SignalTransductions.Regulator.Regulates"是所调控的motif的编号
