# GCModeller
GCModeller Virtual Cell System

GCModeller is original writen in VisualBasic.NET, a feature bioinformatics analysis environment that .NET language hybrids programming with R language was included, which its SDK is available at repository:
https://github.com/SMRUCC/R.Bioinformatics
Currently the R language hybrids programming environment just provides some bioconductor API for the analysis in GCModeller.

GCModeller is a set of utility tools working on the annotation of the whole cell system, this including the whole genome regulation annotation, transcriptome analysis toolkits, metabolism pathway analysis toolkits.

->![Motif analysis based on the meme suite](http://gcmodeller.org/library/assets/TomQuery-example.png)<-
-> *Example of the motif analysis based on the meme suite tools.* <-

Feature tools:
1. NCBI localblast utility
2. MEME tools combine with Regprecise database
3. Circos plots utility
4. Protein structure analysis tools
5. KEGG database tools
6. Reactome/MetaCyc database tools
7. Venn diagram drawing tools
8. Cytoscape utility tools
9. Motif Parallel alignment tools for the protein interaction network and family annotation

The main virtual cell analysis engine is under development progress.

Visit our project home:
http://gcmodeller.org


##For developers
Here are some released library of the GCModeller is published on nuget, then you can install these library in VisualStudio from **Package Manager Console**:

Install Microsoft VisualBasic Runtime environment library for GCModeller:
https://github.com/xieguigang/VisualBasic_AppFramework
>PM>  Install-Package VB_AppFramework

The GCModeller core base library was released:
https://github.com/SMRUCC/GCModeller.Core
>PM>  Install-Package GCModeller.Core

The NCBI localblast analysis toolkit:
https://github.com/SMRUCC/ncbi-localblast
>PM>  Install-Package NCBI_localblast



Copyright(c) SMRUCC 2015. All rights reversed.
