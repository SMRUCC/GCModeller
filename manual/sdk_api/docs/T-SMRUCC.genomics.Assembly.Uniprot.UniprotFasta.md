---
title: UniprotFasta
---

# UniprotFasta
_namespace: [SMRUCC.genomics.Assembly.Uniprot](N-SMRUCC.genomics.Assembly.Uniprot.html)_

A fasta object which is specific for the uniprot fasta title parsing.(专门用于解析Uniprot蛋白质序列记录的Fasta对象)
 
 The following is a description of FASTA headers for UniProtKB (including alternative isoforms), UniRef, UniParc and archived UniProtKB versions. 
 NCBI's program formatdb (in particular its -o option) is compatible with the UniProtKB fasta headers.
 
 UniProtKB
 >db|UniqueIdentifier|EntryName ProteinName OS=OrganismName[ GN=GeneName]PE=ProteinExistence SV=SequenceVersion
 
 Where:
 db Is 'sp' for UniProtKB/Swiss-Prot and 'tr' for UniProtKB/TrEMBL.
 UniqueIdentifier Is the primary accession number of the UniProtKB entry.
 EntryName Is the entry name of the UniProtKB entry.
 ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. 
 In case of multiple SubNames, the first one Is used. The 'precursor' attribute is excluded, 'Fragment' is included with the name if applicable.
 OrganismName Is the scientific name of the organism of the UniProtKB entry.
 GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed.
 ProteinExistence Is the numerical value describing the evidence for the existence of the protein.
 SequenceVersion Is the version number of the sequence.

> http://www.uniprot.org/help/fasta-headers


### Methods

#### CreateObject
```csharp
SMRUCC.genomics.Assembly.Uniprot.UniprotFasta.CreateObject(SMRUCC.genomics.SequenceModel.FASTA.FastaToken)
```
从读取的文件数据之中创建一个Uniprot序列对象

|Parameter Name|Remarks|
|--------------|-------|
|FastaRaw|-|

> 
>  >sp|Q197F8|002R_IIV3 Uncharacterized protein 002R OS=Invertebrate iridescent virus 3 GN=IIV3-002R PE=4 SV=1
>  

#### LoadFasta
```csharp
SMRUCC.genomics.Assembly.Uniprot.UniprotFasta.LoadFasta(System.String)
```
Load the uniprot fasta sequence file.

|Parameter Name|Remarks|
|--------------|-------|
|path|-|



### Properties

#### EntryName
EntryName Is the entry name of the UniProtKB entry.
#### GN
GeneName Is the first gene name of the UniProtKB entry. If there Is no gene name, OrderedLocusName Or ORFname, the GN field Is Not listed. GeneName(基因名称)
#### OrgnsmSpName
OrganismName Is the scientific name of the organism of the UniProtKB entry.
#### PE
ProteinExistence Is the numerical value describing the evidence for the existence of the protein.
#### ProtName
ProteinName Is the recommended name of the UniProtKB entry as annotated in the RecName field. For UniProtKB/TrEMBL entries without a RecName field, the SubName field Is used. 
 In case of multiple SubNames, the first one Is used. The 'precursor' attribute is excluded, 'Fragment' is included with the name if applicable.
#### SV
SequenceVersion Is the version number of the sequence.
#### UniprotID
UniqueIdentifier Is the primary accession number of the UniProtKB entry.
