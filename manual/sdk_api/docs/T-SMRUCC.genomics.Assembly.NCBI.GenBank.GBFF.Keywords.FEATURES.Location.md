---
title: Location
---

# Location
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES](N-SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.html)_






### Properties

#### Complement
这个基因的位置是否在互补链
#### ContiguousRegion
假若目标对象是真核生物基因组的话，则可能会因为内含子的原因出现不连续的片段，故而此时的@"P:SMRUCC.genomics.Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES.Location.Locations"属性会有多个值，这个属性会尝试将连续的区域返回。对于原核生物而言，也可以直接使用这个属性来获取特性位点的在基因组序列之上的位置
#### JoinLocation
对于环状的DNA分子，当某一个特性位点跨越了终点的时候，会有一个这个属性，本属性此时不会为空值
