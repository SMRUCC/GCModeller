# GCModeller <a href="https://gcmodeller.org"><img src="images/logo.png" width="150" align="right" border="1px" ></a>
GCModeller: genomics CAD(Computer Assistant Design) Modeller system in .NET language

[![DOI](https://zenodo.org/badge/48901128.svg)](https://zenodo.org/badge/latestdoi/48901128)
[![Github All Releases](https://img.shields.io/github/downloads/SMRUCC/GCModeller/total.svg?maxAge=2592000?style=flat-square)]()
[![GPL Licence](https://badges.frapsoft.com/os/gpl/gpl.svg?v=103)](https://opensource.org/licenses/GPL-3.0/)
[![Build Status](https://travis-ci.org/SMRUCC/GCModeller.svg?branch=master)](https://travis-ci.org/SMRUCC/GCModeller)

###### [WARNING] This project is a work in progress and is not recommended for production use.

> + HOME: http://gcmodeller.org
> + Github: https://github.com/SMRUCC/GCModeller
> + BioTools: https://bio.tools/gcmodeller

**Supported platform:** ``Microsoft Windows``, ``GNU Linux``, ``MAC``, ``Microsoft Azure Cloud`` <br />
**Development:** Microsoft VisualStudio 2019 | VisualBasic.NET<br />
**Runtime environment:** [sciBASIC#](https://www.nuget.org/packages/sciBASIC/) v2.1.5 beta &amp; ``.NET Framework 4.7`` (or ``mono 6.4``)<br />
**Installation:** ``VS2019`` is required of compiles this project. After the source code have been clone using git, just open solution file **[/src/GCModeller.sln](./src/GCModeller.sln)**, and when restore nuget packages finished, then you are good to go of compile GCModeller project.<br />

> **NOTE:** Due to the reason of this project is using git submodule for manage some runtime component, so that **please do not directly download** the project source code from github by using the ``Donwload Zip`` button. The internal github client in the VisualStudio is recommended using for download the project source code.

#### Docker and Database

**Dependency:** Part of the GCModeller function required running Linux tools through [Darwinism](https://github.com/xieguigang/Darwinism) Docker environment for VB.NET (If you are running GCModeller on Windows Platform). This toolkit required of these environment installed on your windows server:

 + Microsoft PowerShell SDK 3.0
 + Latest version of Docker for X64
 + Then pull environment container image via: ``docker pull xieguigang/gcmodeller-env``.

The docker container image contains these utils that required by GCModeller:

> + MEME suite for motif analysis
> + Mothur for construct OTU

**Install Database:** Some feature in GCModeller required the fasta sequence database was installed on a specific location on your server's filesystem, please follow this [instruction]() to install the database for GCModeller.

---------------------------------------------------------------------------------------------------

<img src="http://gcmodeller.org/DNA.png" width=40 height=48 />``GCModeller`` is an open source cloud computing platform for the geneticist and systems biology. You can easily build a local computing server cluster for ``GCModeller`` on the large amount biological data analysis.

The ``GCModeller`` platform is original writen in ``VisualBasic.NET`` language, a feature bioinformatics analysis environment that .NET language hybrids programming with R language was included, which its SDK is available at repository:
https://github.com/SMRUCC/R.Bioinformatics

Currently the ``R`` language hybrids programming environment just provides some ``bioconductor`` API for the analysis in ``GCModeller``.

``GCModeller`` is a set of utility tools working on the annotation of the whole cell system, this including the whole genome regulation annotation, transcriptome analysis toolkits, metabolism pathway analysis toolkits. And some common bioinformatics problem utils tools and common biological database I/O tools is also available in GCModeller for the .NET language programming.

### Directory Roadmap
+ [/GCModeller](./GCModeller) : The location of GCModeller compile output, I have config all of the project output in the path ``./GCModeller/bin/``
+ [/src](./src)
	+ [/src/GCModeller](./src/GCModeller) : GCModeller basic library and analysis protocols
	+ [/src/interops](./src/interops) : GCModeller tools that dependent on the external programs
	+ [/src/R.Bioconductor](./src/R.Bioconductor) : R language hybrids environment
	+ [/src/R-sharp](./src/R-sharp) : The GCModeller R# language scripting engine
	+ [/src/repository](./src/repository) : GCModeller data repository system
	+ [/src/runtime](./src/runtime) : Third part library and VisualBasic runtime source code
+ [/tools](./tools)

### Data Standards
+ GCModeller supports the ``SBML`` and ``BIOM`` data standards for exchanges the analysis and model data with other bioinformatics softwares.
+ Supports ``PSI`` data for the biological interaction network model
+ Supports ``OBO`` data for ontology database like ``go``.

<a href="http://sbml.org/Main_Page"><img src="./src/GCModeller/models/images/sbml-logo-70.png" width=80></a> <a href="http://biom-format.org/"><img src="./src/GCModeller/models/images/biom-format.png" width=80></a> <a href="http://www.psidev.info/overview"><img src="./images/data_standards/PSI_logo_s.png" width=80></a> <a href="http://www.obofoundry.org/"><img src="./images/data_standards/foundrylogo.png" width=80></a>

### Modules &amp; Functions

GCModeller provides a set of .NET libraries and CLI tools for processing biological analysis data. Currently **GCModeller** can provides these productive ready libraries:

###### 1.Basically libraries

+ NCBI data analysis toolkit: Genbank/Taxonomy/nt/nr database
+ Common Data: FastA database, FastQ, SAM data file I/O class
+ Biological Data Standard Supports: SBML(level 3), BIOM(level1), PSI, OBO
+ Biological Pathway Database: MetaCyc, Reactome, KEGG data tools for .NET language

###### 2.Biological Data Visualization software API for .NET

+ Circos API(genomic visualizing)
+ Cytoscape DataModel API(Biological network visualizing)
+ SequenceLogo(Molecular motif site visualize)
+ KEGG pathway map visualizer

###### 3.Annotation Tools:

+ A complete NCBI localblast toolkit for proteins and nucleotide sequence analysis, includes parallel task library for Win/Linux Server and data analysis protocol.
+ SNP toolkit
+ Nucleotide sequence topology feature site analysis toolkit.
+ RegPrecise database tool and MEME software toolkit for the annotation of bacterial genomics regulation network.
+ Go(Gene Ontology) annotation tools
+ KEGG/GO GSEA functional enrichment tools and reference genome background model creator based on UniProt database.

###### 4.R language hybrids environment for bioinformatics

+ Includes basically R language API wrapper for VisualBasic, like Api in ``base``, ``utils``, ``stats`` namespace from R base.
+ and some R package wrapper API from CRAN and Bioconductor is also included.
+ GCModeller ``R#`` language scripting

###### 5.WebAPI wrapper for KEGG database and RegPrecise Database

###### 6.Feature tools 

+ Cellular module simulator, and virtual cell model generator protocol.
+ Proteomics data analysis toolkit
+ Single-cell data analysis toolkit

### GCModeller R# scripting

Here is a code snapshot of R# scripting for drawing sequence logo, input data is accepted from the commandline input:  

```R
# Demo script for create sequence logo based on the MSA alignment analysis
# nt base frequency is created based on the MSA alignment operation.

imports "bioseq.sequenceLogo" from "seqtoolkit.dll";
imports "bioseq.fasta" from "seqtoolkit.dll";

# script cli usage
#
# R# sequenceLogo.R --seq input.fasta [--title <logo.title> --save output.png] 
#

# get input data from commandline arguments and
# fix for the optional arguments default value
# by apply or default syntax for non-logical values
let seq.fasta as string = ?"--seq"   || stop("No sequence input data for draw sequence logo!");
let logo.png as string  = ?"--save"  || `${seq.fasta}.logo.png`;
let title as string     = ?"--title" || basename(seq.fasta);

# read sequence and then do MSA alignment
# finally count the nucleotide base frequency
# and then draw the sequence logo
# by invoke sequence logo drawer api
seq.fasta
:> read.fasta
:> MSA.of
:> plot.seqLogo(title)
:> save.graphics( file = logo.png );
```

Run the R# script from commandline:

```cmd
@echo off

R# ./sequenceLogo.R --seq LexA.fasta --save LexA.png --title "LexA"
```

![](images/LexA.png)

### Publications

Here listing the scientific paperworks that based on the analysis services of GCModeller:

+ _Niu, X.-N., et al. (2015)._ **"Complete sequence and detailed analysis of the first indigenous plasmid from Xanthomonas oryzae pv. oryzicola."** BMC Microbiol 15(1): 1-15.

    + **DOI:** 10.1186/s12866-015-0562-x

> Bacterial plasmids have a major impact on metabolic function and adaptation of their hosts. An indigenous plasmid was identified in a Chinese isolate (GX01) of the invasive phytopathogen Xanthomonas oryzae pv. oryzicola (Xoc), the causal agent of rice bacterial leaf streak (BLS). To elucidate the biological functions of the plasmid, we have sequenced and comprehensively annotated the plasmid.

![](./2016-05-17.png)

## Gallery

![](./src/GCModeller/analysis/SingleCell/STdeconvolve/demo/raw/pixels.png)

single cell data toolkit includes in GCModeller:

+ [PhenoGraph](./src/GCModeller/analysis/SingleCell/PhenoGraph/PhenoGraph)
+ [STdeconvolve](./src/GCModeller/analysis/SingleCell/STdeconvolve/STdeconvolve)

![](./images/cmeans_keggSet.png)
![](./images/upsetPlot.png)
[![](./src/workbench/R%23/demo/HTS/patterns.png)](./src/workbench/R%23/demo/HTS/expression_patterns.R)
![](./images/vocano.plot.png)
![](./images/KEGG-pathway-network-clusters.png)
![](./images/RSD-P.density.png)
![](./images/clusters.scatter.png)
![](./images/xcb_TCS_uniprot-taxonomy_314565_GO_enrichment_converts.GO_enrichment.pvalue=0.05.png)
<img src="images/biological_process.png" width=285 />
<img src="images/cellular_component.png" width=285 />
<img src="images/molecular_function.png" width=285 />
<img src="./manual/KEGG/Unigenes.blast.m8.filter-KO.Catalogs/kegg-level-A.png" width=435 /><img src="/images/GO-enrichment.png" width=435 />
![](./images/FUR-lightbox.png)
![](./images/Xanthomonas_oryzae_oryzicola_BLS256_uid16740-lightbox.png)
![](./images/pXOCGX01-lightbox.png)
![](./images/phenotypic-bTree-lightbox.png)
![](./images/pxocgx01_blastx-lightbox.png)

Visit our project home: http://gcmodeller.org

## For developers
Here are some released library of the ``GCModeller`` is published on nuget, then you can install these library in ``VisualStudio`` from **Package Manager Console**:

```bash
# Install Microsoft VisualBasic sciBASIC# runtime via nuget:
# https://github.com/xieguigang/sciBASIC/
PM>  Install-Package sciBASIC -Pre

# The GCModeller core base library was released:
# https://github.com/SMRUCC/GCModeller.Core
PM>  Install-Package GCModeller.Core

# The NCBI localblast analysis toolkit:
# https://github.com/SMRUCC/ncbi-localblast
PM>  Install-Package NCBI_localblast
```

## For User

The GCModeller demo script and data for user tutorials can be download from these public data repository:
+ [[Xanthomonas campestris pv. campestris 8004] GCModeller genomics Modelling Project](https://github.com/SMRUCC/Xanthomonas_campestris_8004_uid15).

###### genome map plot of Xanthomonas campestris pv. campestris 8004

[![](https://raw.githubusercontent.com/SMRUCC/Xanthomonas_campestris_8004_uid15/master/thumbnails/map-part-A.png)](https://github.com/SMRUCC/Xanthomonas_campestris_8004_uid15/tree/master/genome/chromsome-map)
[![](https://raw.githubusercontent.com/SMRUCC/Xanthomonas_campestris_8004_uid15/master/thumbnails/map-part-B.png)](https://github.com/SMRUCC/Xanthomonas_campestris_8004_uid15/tree/master/genome/chromsome-map)

----------------------------------------------------------------------------------------------------
<img src="./images/links/osi-certified.png" width="40px"/> ![](./images/links/github.ico)

Copyleft &copy; [SMRUCC genomics](http://smrucc.org) 2016. All rights reversed.
