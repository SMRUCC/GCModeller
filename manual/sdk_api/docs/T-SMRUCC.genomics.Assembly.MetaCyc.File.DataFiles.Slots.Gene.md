---
title: Gene
---

# Gene
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_






### Properties

#### Accession1
The unique identifier of this gene object in the NCBI genbak database.
 (本基因对象在NCBI Genbak数据库之中的唯一标识符)
#### CentisomePosition
This slot lists the map position of this gene on the chromosome in centisome units 
 (percentage length of the chromosome). The centisome-position values are computed 
 automatically by Pathway Tools from the Left-End-Position slot. The value is a number 
 between 0 and 100, inclusive.
#### Interrupted
If True, indicates that the specified gene is interrupted, that is, has a premature stop codon.
#### LeftEndPosition
These slots encode the position of the left and right ends of the gene on the 
 chromosome or plasmid on which the gene resides. "Left" means the end of the 
 gene toward the coordinate-system origin (0). Therefore, the Left-End-Position 
 is always less than the Right-End-Position.
#### Product
This slot holds the ID of a polypeptide or tRNA frame, which is the product of this gene. 
 This slot may contain multiple values for two possible reasons: a given gene might be 
 translated from more than one start codon, giving rise to products of different lengths; 
 the product of the gene may undergo chemical modification. In the latter case, the gene 
 lists all modified forms of the protein in its Product slot.(对于MetaCyc数据库而言，本属性
 值包含有所有类型的蛋白质对象的UniqueID，但是在编译后的计算机模型之中，仅包含有不同启动子而形成
 的所有不同长度的多肽链)
#### RightEndPosition
These slots encode the position of the left and right ends of the gene on the 
 chromosome or plasmid on which the gene resides. "Left" means the end of the 
 gene toward the coordinate-system origin (0). Therefore, the Left-End-Position 
 is always less than the Right-End-Position.
#### TranscriptionDirection
This slot specifies the direction along the chromosome in which this gene is transcribed; 
 allowable values are "+" and "-".
