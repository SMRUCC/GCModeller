---
title: GeneBrief
---

# GeneBrief
_namespace: [SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels](N-SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.html)_

The gene brief information data in a ncbi PTT document.(PTT文件之中的一行，即一个基因的对象摘要信息)



### Methods

#### DocumentParser
```csharp
SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.ComponentModels.GeneBrief.DocumentParser(System.String)
```
42..1370+44266766353dnaAXC_0001-COG0593Lchromosome replication initiator DnaA

|Parameter Name|Remarks|
|--------------|-------|
|strLine|-|



### Properties

#### Gene
基因名，在genbank文件里面是/gene=，基因号，这个应该是GI编号，而非平常比较熟悉的字符串编号
#### IsBlankSegment
判断本对象是否是由@"M:SMRUCC.genomics.ContextModel.LocationDescriptions.BlankSegment``1(SMRUCC.genomics.ComponentModel.Loci.NucleotideLocation)"方法所生成的空白片段
#### IsORF
*.ptt => TRUE; *.rnt => FALSE
#### Length
The NT length of this ORF.
#### Location
The location of this ORF gene on the genome sequence.(包含有PTT文件之中的Location, Strand和Length列)
#### Product
Protein product functional description in the genome.
 (基因的蛋白质产物的功能的描述)
#### Synonym
The gene's locus_tag data.
 (我们所正常熟知的基因编号，@"T:SMRUCC.genomics.Assembly.NCBI.GenBank.TabularFormat.PTT"对象主要是使用这个属性值来生成字典对象的)
