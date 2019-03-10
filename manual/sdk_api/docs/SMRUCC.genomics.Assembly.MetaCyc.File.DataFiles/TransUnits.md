﻿# TransUnits
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles](./index.md)_

Frames in this class encode transcription units, which are defined as a set of genes and
 associated control regions that produce a single transcript. Thus, there is a one-to-one
 correspondence between transcription start sites and transcription units. If a set of genes
 is controlled by multiple transcription start sites, then a PGDB should define multiple
 transcription-unit frames, one for each transcription start site.
 (在本类型中所定义的对象编码一个转录单元，一个转录单元定义了一个基因及与其相关联的转录调控DNA片段
 的集合，故而，在本对象中有一个与转录单元相一一对应的转录起始位点。假若一个基因簇是由多个转录起始
 位点所控制的，那么将会在MetaCyc数据库之中分别定义与这些转录起始位点相对应的转录单元【即，每一个
 本类型的对象的属性之中，仅有一个转录起始位点属性】)




### Properties

#### AttributeList
定义在转录单元类型之中的所有的属性的列表
