---
title: PerlInvoke
---

# PerlInvoke
_namespace: [SMRUCC.genomics.Interops.CARMEN](N-SMRUCC.genomics.Interops.CARMEN.html)_

The software CARMEN was developed to support functional and comparative genome analysis. 
 CARMEN provides the visualization of automatically obtained metabolic networks based on 
 KEGG database information and stores the generated data in standardized SBML format. 
 SBML is an open source XML-based format that facilitates the description of models and 
 their exchange between various simulation and analysis tools (Hucka, 2003).



### Methods

#### KGMLReconstruct
```csharp
SMRUCC.genomics.Interops.CARMEN.PerlInvoke.KGMLReconstruct(System.String,System.String[],System.String,System.Int32,System.String,System.Boolean,System.Boolean,System.Boolean,System.Int32)
```
Perl-Programm to reconstruct metabolic pathways based on KGML files of the KEGG database.

|Parameter Name|Remarks|
|--------------|-------|
|g|-g      Name of the Genbank file|
|l|-l      File containing tab-separated EC number list|
|o|-o      Name of the SBML-output-file|
|n|-n      Number of the columns of the sbml file|
|k|-k      Id's of the kegg-maps, for example 00010|
|m|-m      include maplinks, add T for true (default = F)|
|c|-c      Cofactor integration, add T for true (default = F)|
|e|-e      EC number joining, add T for true (default = F)|
|f|-f      output format, 1=SBML 2.1 (for CellDesigner), 2=SBML2.4; 3=SBML 2.4 (for CellDesigner); 4=SBML 3.1; A=all (default = 3)|



### Properties

#### KGML
KGML_reconstruction.pl
