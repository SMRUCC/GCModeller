![](https://cdn.rawgit.com/LunaGao/BlessYourCodeTag/master/tags/alpaca.svg)
[![Github All Releases](https://img.shields.io/github/downloads/SMRUCC/GCModeller/total.svg?maxAge=2592000?style=flat-square)]()
[![GPL Licence](https://badges.frapsoft.com/os/gpl/gpl.svg?v=103)](https://opensource.org/licenses/GPL-3.0/)

# GCModeller
GCModeller Virtual Cell System

GCModeller is an open source cloud computing platform for the geneticist and systems biology. You can easily build a local computing server cluster for GCModeller on the large amount biological data analysis. 

The GCModeller platform is original writen in VisualBasic.NET, a feature bioinformatics analysis environment that .NET language hybrids programming with R language was included, which its SDK is available at repository:
https://github.com/SMRUCC/R.Bioinformatics
Currently the R language hybrids programming environment just provides some bioconductor API for the analysis in GCModeller.

GCModeller is a set of utility tools working on the annotation of the whole cell system, this including the whole genome regulation annotation, transcriptome analysis toolkits, metabolism pathway analysis toolkits.

#### Gallery
![](https://raw.githubusercontent.com/SMRUCC/GCModeller/master/2016-05-17.png)
![](https://raw.githubusercontent.com/SMRUCC/GCModeller/master/images/FUR-lightbox.png)
![](https://raw.githubusercontent.com/SMRUCC/GCModeller/master/images/Xanthomonas_oryzae_oryzicola_BLS256_uid16740-lightbox.png)
![](https://raw.githubusercontent.com/SMRUCC/GCModeller/master/images/pXOCGX01-lightbox.png)
![](https://raw.githubusercontent.com/SMRUCC/GCModeller/master/images/phenotypic-bTree-lightbox.png)
![](https://raw.githubusercontent.com/SMRUCC/GCModeller/master/images/pxocgx01_blastx-lightbox.png)

##Feature tools
*  1. NCBI localblast utility
*  2. MEME tools combine with Regprecise database
*  3. Circos plots utility
*  4. Protein structure analysis tools
*  5. KEGG database tools
*  6. Reactome/MetaCyc database tools
*  7. Venn diagram drawing tools
*  8. Cytoscape utility tools
*  9. Motif Parallel alignment tools for the protein interaction network and family annotation

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

https://raw.githubusercontent.com/SMRUCC/GCModeller/master/svn_history.txt

Copyright(c) SMRUCC 2015. All rights reversed.
