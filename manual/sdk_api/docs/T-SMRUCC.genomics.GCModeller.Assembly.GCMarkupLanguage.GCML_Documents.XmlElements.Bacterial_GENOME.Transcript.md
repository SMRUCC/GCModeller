---
title: Transcript
---

# Transcript
_namespace: [SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME](N-SMRUCC.genomics.GCModeller.Assembly.GCMarkupLanguage.GCML_Documents.XmlElements.Bacterial_GENOME.html)_

每一种RNA分子仅生成一种产物分子




### Properties

#### CompositionVector
核酸碱基构成，ATGC
#### Product
UniqueId of the protein element in the metabolites list collection.
 (指向Metabolites列表中的蛋白质对象的UniqueId)
 【This slot holds the ID of a polypeptide or tRNA frame, which is the product of this gene. 
 This slot may contain multiple values for two possible reasons: a given gene might be 
 translated from more than one start codon, giving rise to products of different lengths; 
 the product of the gene may undergo chemical modification. In the latter case, the gene 
 lists all modified forms of the protein in its Product slot.】
 【这个属性值为本基因的表达产物：一个多肽链单体蛋白或者tRNA分子的UniqueId属性值，本属性由于两个原因
 可能包含有多个值：
 1. 基因可能从不同的翻译起始密码子开始翻译，从而产生不同长度的产物；
 2. 基因的产物可能在经过化学修饰，当为这种情况的时候，本属性将会列举出蛋白质产物的所有修饰形式】
 (对于MetaCyc数据库而言，本属性值包含有所有类型的蛋白质对象的UniqueID，但是在编译后的计算机模型之中，
 仅包含有不同启动子而形成的所有不同长度的多肽链)
