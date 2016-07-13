---
title: COGs
---

# COGs
_namespace: [SMRUCC.genomics.Assembly.NCBI.COG.COGs](N-SMRUCC.genomics.Assembly.NCBI.COG.COGs.html)_

############################################################
 0.General remarks
 ############################################################
 
 This Is a December 2014 release of 2003-2014 COGs constructed by
 Eugene Koonin 's group at the National Center for Biotechnology
 Information (NCBI), National Library of Medicine (NLM), National
 Institutes of Health (NIH).
 
 #-----------------------------------------------------------
 0.1.Citation
 
 Galperin MY, Makarova KS, Wolf YI, Koonin EV.
 
 Expanded microbial genome coverage And improved protein family
 annotation in the COG database.
 
 Nucleic Acids Res. 43, D261-D269, 2015
 <http: //www.ncbi.nlm.nih.gov/pubmed/25428365>
 
 #-----------------------------------------------------------
 0.2.Contacts
 
 <COGsncbi.nlm.nih.gov>
 
 ############################################################
 1.Notes
 ############################################################
 
 #-----------------------------------------------------------
 1.1.2003-2014 COGs
 
 This release contains 2003 COGs assigned To a representative Set Of
 bacterial And archaeal genomes, available at February 2014. No New
 COGs were constructed.
 
 #-----------------------------------------------------------
 1.2.GIs And Refseq IDs
 
 Sequences in COGs are identified by GenBank GI numbers. GI numbers
 generally are transient. There are two ways To make a more permanent
 link between the protein In COGs And the outside databases: via the
 RefSeq accession codes (see 2.5) And via the protein sequences (see
 2.6).
 
 Note, however, that at the moment (April 02, 2015) RefSeq database Is
 in a state of transition; some of the <refseq-acc> entries are Not
 accessible. This accession table will be updated as soon as RefSeq Is
 stable.



### Methods

#### GroupRelease
```csharp
SMRUCC.genomics.Assembly.NCBI.COG.COGs.COGs.GroupRelease(System.String)
```
将prot2003-2014.fasta按照@"P:SMRUCC.genomics.Assembly.NCBI.COG.COGs.ProtFasta.GenomeName"分组导出，以方便使用bbh进行注释分析

|Parameter Name|Remarks|
|--------------|-------|
|Fasta|-|


#### SaveRelease
```csharp
SMRUCC.genomics.Assembly.NCBI.COG.COGs.COGs.SaveRelease(System.String,System.String)
```


|Parameter Name|Remarks|
|--------------|-------|
|Fasta|prot2003-2014.fasta|
|Export|数据按照基因组分组到处的结果所保存的文件夹|



