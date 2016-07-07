---
title: Metabolite
---

# Metabolite
_namespace: [SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream](N-SMRUCC.genomics.GCModeller.ModellingEngine.Assembly.GCTabular.FileStream.html)_

代谢物对象，当前的这个类型的对象是GCModeller计算引擎内的所有的模拟基础




### Properties

#### InitialAmount
本代谢物对象在系统初始化的时候的初始数量
#### n_FluxAssociated
与本代谢物相关的流对象的数目，计算规则：
 当处于不可逆反应的时候：处于产物边，计数为零，处于底物边，计数为1
 当处于可逆反应的时候：无论是处于产物边还是底物边，都被计数为0.5
