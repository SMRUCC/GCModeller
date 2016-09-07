![](https://cdn.rawgit.com/LunaGao/BlessYourCodeTag/master/tags/alpaca.svg)
[![Github All Releases](https://img.shields.io/github/downloads/SMRUCC/GCModeller/total.svg?maxAge=2592000?style=flat-square)]()
[![GPL Licence](https://badges.frapsoft.com/os/gpl/gpl.svg?v=103)](https://opensource.org/licenses/GPL-3.0/)
[![Build Status](https://travis-ci.org/SMRUCC/GCModeller.svg?branch=master)](https://travis-ci.org/SMRUCC/GCModeller)

# GCModeller
GCModeller: genomics CAD(Computer Assistant Design) Modeller system in .NET language

> + HOME: http://gcmodeller.org
> + Online services: http://services.gcmodeller.org
> + Github: https://github.com/smrucc/GCModeller
> + SDK docs: http://docs.gcmodeller.org

**Supported platform:** ``Microsoft Windows``, ``GNU Linux``, ``MAC`` <br />
**Development:** Microsoft VisualStudio 2015 | VisualBasic.NET<br />
**Runtime environment:** [VisualBasic App](https://www.nuget.org/packages/VB_AppFramework/) v1.0.40 &amp; ``.NET Framework 4.6`` (or ``mono 4.4``)

<img src="http://gcmodeller.org/DNA.png" width=40 height=48 />``GCModeller`` is an open source cloud computing platform for the geneticist and systems biology. You can easily build a local computing server cluster for ``GCModeller`` on the large amount biological data analysis.

The ``GCModeller`` platform is original writen in ``VisualBasic.NET`` language, a feature bioinformatics analysis environment that .NET language hybrids programming with R language was included, which its SDK is available at repository:
https://github.com/SMRUCC/R.Bioinformatics
Currently the R language hybrids programming environment just provides some ``bioconductor`` API for the analysis in ``GCModeller``.

``GCModeller`` is a set of utility tools working on the annotation of the whole cell system, this including the whole genome regulation annotation, transcriptome analysis toolkits, metabolism pathway analysis toolkits. And some common bioinformatics problem utils tools and common biological database I/O tools is also available in GCModeller for the .NET language programming.

#### Data Standards
+ GCModeller supports the ``SBML`` and ``BIOM`` data standards for exchanges the analysis and model data with other bioinformatics softwares.
+ Supports ``PSI`` data for the biological interaction network model
+ Supports ``OBO`` data for ontology database like ``go``.

<a href="http://sbml.org/Main_Page"><img src="https://raw.githubusercontent.com/xieguigang/GCModeller/master/src/GCModeller/models/images/sbml-logo-70.png" width=80></a> <a href="http://biom-format.org/"><img src="https://raw.githubusercontent.com/xieguigang/GCModeller/master/src/GCModeller/models/images/biom-format.png" width=80></a> <a href="http://www.psidev.info/overview"><img src="./images/data_standards/PSI_logo_s.png" width=80></a> <a href="http://www.obofoundry.org/"><img src="./images/data_standards/foundrylogo.png" width=80></a>

#### Gallery

![](https://raw.githubusercontent.com/SMRUCC/GCModeller/master/2016-05-17.png)
![](./images/3d_bio_net_canvas.gif)
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
Here are some released library of the ``GCModeller`` is published on nuget, then you can install these library in ``VisualStudio`` from **Package Manager Console**:

Install Microsoft VisualBasic Runtime environment library for GCModeller:
https://github.com/xieguigang/VisualBasic_AppFramework
>PM>  Install-Package VB_AppFramework

The GCModeller core base library was released:
https://github.com/SMRUCC/GCModeller.Core
>PM>  Install-Package GCModeller.Core

The NCBI localblast analysis toolkit:
https://github.com/SMRUCC/ncbi-localblast
>PM>  Install-Package NCBI_localblast

-------------------------------------------------------------------------------------------------------------------------------

Copyright &copy; [SMRUCC](http://smrucc.org) 2015. All rights reversed.
