---
title: Protein
---

# Protein
_namespace: [SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots](N-SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.html)_

The class of all proteins is divided into two subclasses: protein complexes and polypeptides.
 A polypeptide is a single amino acid chain produced from a single gene. A protein
 complex is a multimeric aggregation of more than one polypeptide subunit. A protein
 complex may in some cases have another protein complex as a component. Many of the
 slots that are applicable to Proteins are also applicable to members of the RNA class.
 (本类型的对象会枚举所有的Component对象的UniqueID)

> 
>  Protein表对象和ProtLigandCplxe表对象相比较：
>  Protein表中包含有所有类型的蛋白质对象，而ProtLigandCplxe则仅包含有蛋白质和小分子化合物配合的之后所形成的复合物，
>  所以基因的产物在ProtLigandCplxe表中是无法找到的
>  


### Methods

#### New
```csharp
SMRUCC.genomics.Assembly.MetaCyc.File.DataFiles.Slots.Protein.New
```
返回一个新构造出来的Protein对象


### Properties

#### Catalyzes
A list of enzymatic reaction unique id that catalyzed by this protein.(本蛋白质所催化的酶促反应的UniqueId的列表)
#### ComponentOf
This slot lists the complex(es) that this protein is a component of, if any, including protein
 complexes, protein-small-molecule complexes, protein-RNA complexes, and so on.
#### DNAFootprintSize
For proteins that bind to DNA, the number of base pairs on the DNA strand that the
 binding protein covers.
#### Gene
The gene's UniqueId that indicated that which gene codes this polypeptide.
#### GoTerms
Values of this slot are the Gene Ontology terms to which this object is annotated. Each
 value should be annotated with citations, including evidence codes.
#### IsModifiedProtein
判断本蛋白质对象是否为经过化学修饰的蛋白质
#### IsPolypeptide
判断本蛋白质对象是否为一个多肽链对象
#### Locations
This slot describes the one or more cellular locations in which this protein is found. It’s
 values are members of the CCO (Cell Component Ontology) class.
#### ModifiedForm
This slot points from the unmodified form of a protein to one or more chemically modified
 forms of that protein. For example, the slot might point from the unmodified form of
 a polypeptide (or a protein complex) to a phosphorylated form of that polypeptide (or
 protein complex).
#### MolecularWeightExp
This slot lists the molecular weight of the protein complex or polypeptide, derived experimentally.
 Multiple values of this slot correspond to multiple experimental observations.
 Units: kilodaltons.
#### MolecularWeightKD
This computed slot lists the known molecular weight(s) of a macromolecule by taking the
 union of the slots Molecular-Weight-Seq and Molecular-Weight-Exp. Units: kilodaltons.
#### MolecularWeightSeq
This slot lists the molecular weight of the protein complex or polypeptide, as derived
 from sequence data. Units: kilodaltons.
#### pI
This slot lists the pI of the polypeptide.
#### Regulates
For proteins that have regulatory activity (e.g. as transcription factors), this slot points to
 the Regulation frames that describe the regulation and link to the regulated entity.
#### Species
This slot is used in proteins only in the MetaCyc DB, in which case it identifies the species
 in which the current protein is found.
#### UnmodifiedForm
This slot points from a chemically modified form of some protein, to the native unmodified
 form of that protein (e.g., from a phosphorylated form to the unphosphorylated
 form).
