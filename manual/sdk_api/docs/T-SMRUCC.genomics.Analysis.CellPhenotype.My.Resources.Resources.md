---
title: Resources
---

# Resources
_namespace: [SMRUCC.genomics.Analysis.CellPhenotype.My.Resources](N-SMRUCC.genomics.Analysis.CellPhenotype.My.Resources.html)_

A strongly-typed resource class, for looking up localized strings, etc.




### Properties

#### Culture
Overrides the current thread's CurrentUICulture property for all
 resource lookups using this strongly typed resource class.
#### ExpressionMAT
Looks up a localized string similar to imports Analysis_Tools.Phenotype.Regulations
imports Framework.Extensions

$dir
$gene

csv <- $dir -> expressionmatrix.create
$csv > ./Export/$gene.MAT.csv
 .
#### KernelDriver
Looks up a localized string similar to Imports Analysis_Tools.Phenotype.Regulations
Imports MEME.app.genome_Footprints
Imports GCModeller.Compiler.GCML.Csvx
Imports IO_Device.Csv
Imports GCModeller.Assembly.File.IO

# This script file is using for running the GCModeller DFL binary network engine kernel.

# ----------------------------------------------------------------------------------------------------------
# Script file input parameters
# ------------------------------------------------------------------------------------- [rest of string was truncated]";.
#### KernelDriver___GeneMutation__MonteCarlo
Looks up a localized string similar to Imports Analysis_Tools.Phenotype.Regulations
Imports MEME.app.genome_Footprints
Imports GCModeller.Compiler.GCML.Csvx
Imports IO_Device.Csv
Imports GCModeller.Assembly.File.IO

# This script file is using for running the GCModeller DFL binary network engine kernel.

# ----------------------------------------------------------------------------------------------------------
# Script file input parameters
# ------------------------------------------------------------------------------------- [rest of string was truncated]";.
#### pfsnet_kegg_pathways
Looks up a localized string similar to imports assemblyfile.io
imports analysis.chipdata
imports pfsnet
imports kegg

$chipdata
$save
pathways <- "[pathways]"
java_filter <- "[java_filter]"

$pathways ~ read.csv.kegg_pathways

call pfsnet session.initialize $java_filter

chipdata <- read.chipdata $chipdata

experiment1 <- try_parse_string_vector "1,2,3,4,5,6,7,8,9,10"
experiment2 <- try_parse_string_vector "11,12,13,14,15,16,17,18,19,20"

geneidlist <- pathway_genelist.create_from pathwaydata $pathways
file1 <- expression_mat [rest of string was truncated]";.
#### PfsNETCellularNetwork
Looks up a localized string similar to Imports PfsNET

# Set up data files in the program with String.Format function
 MAT1 <- {0}
 MAT2 <- {1} 
 NET <- {2}

# PfsNET evaluation parameters 
 $Beta
 $t1
 $t2
 $n

Call Session.Initialize
 
# Invoke the PfsNET evaluation and save data to file
Result <- PfsNET.Evaluate File1 $MAT1 File2 $MAT2 File3 $NET b $Beta t1 $t1 t2 $t2 n $n
$Result > {3}/{4}.xml.
#### ResourceManager
Returns the cached ResourceManager instance used by this class.
