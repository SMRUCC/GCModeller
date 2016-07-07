---
title: FBA
tags: [maunal, tools]
date: 7/7/2016 6:51:31 PM
---
# GCModeller [version 1.1.24.3]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/FBA.exe
**Root namespace**: FBA.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|/Analysis.Phenotype||
|/Export||
|/Flux.Coefficient||
|/Flux.KEGG.Filter||
|/Func.Coefficient||
|/gcFBA.Batch||
|/heatmap|Draw heatmap from the correlations between the genes and the metabolism flux.|
|/heatmap.scale||
|/Imports||
|/phenos.MAT|Merges the objective function result as a Matrix. For calculation the coefficient of the genes with the phenotype objective function.|
|/phenos.out.Coefficient|2. Coefficient of the genes with the metabolism fluxs from the batch analysis result.|
|/phenos.out.MAT|1. Merge flux.csv result as a Matrix, for the calculation of the coefficient of the genes with the metabolism flux.|
|/Solve|solve a FBA model from a specific (SBML) model file.|
|/Solver.KEGG||
|/Solver.rFBA||
|compile|Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.|

## Commands
--------------------------
##### Help for command '/Analysis.Phenotype':

**Prototype**: FBA.CLI::Int32 rFBABatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe /Analysis.Phenotype /in <MetaCyc.Sbml> /reg <footprints.csv> /obj <list/path/module-xml> [/obj-type <lst/pathway/module> /params <rfba.parameters.xml> /stat <stat.Csv> /sample <sampleTable.csv> /modify <locus_modify.csv> /out <outDIR>]
  Example:      FBA /Analysis.Phenotype 
```

##### Help for command '/Export':

**Prototype**: FBA.CLI::Int32 Export(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe export -i <fba_model> -o <r_script>
  Example:      FBA /Export export -i /home/xieguigang/ecoli.xml -o /home/xieguigang/ecoli.r
```

##### Help for command '/Flux.Coefficient':

**Prototype**: FBA.CLI::Int32 FluxCoefficient(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe /Flux.Coefficient /in <rFBA.result_dumpDIR> [/footprints <footprints.csv> /out <outCsv> /spcc /KEGG]
  Example:      FBA /Flux.Coefficient 
```

##### Help for command '/Flux.KEGG.Filter':

**Prototype**: FBA.CLI::Int32 KEGGFilter(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe /Flux.KEGG.Filter /in <flux.csv> /model <MetaCyc.sbml> [/out <out.csv>]
  Example:      FBA /Flux.KEGG.Filter 
```

##### Help for command '/Func.Coefficient':

**Prototype**: FBA.CLI::Int32 FuncCoefficient(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe /Func.Coefficient /func <objfunc_matrix.csv> /in <rFBA.result_dumpDIR> [/footprints <footprints.csv> /out <outCsv> /spcc]
  Example:      FBA /Func.Coefficient 
```

##### Help for command '/gcFBA.Batch':

**Prototype**: FBA.CLI::Int32 PhenotypeAnalysisBatch(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe /gcFBA.Batch /model <model.sbml> /phenotypes <KEGG_modules/pathways.DIR> /footprints <footprints.csv> [/obj-type <pathway/module> /params <rfba.parameters.xml> /stat <RPKM-stat.Csv> /sample <sampleTable.csv> /modify <locus_modify.csv> /out <outDIR> /parallel <2>]
  Example:      FBA /gcFBA.Batch 
```

##### Help for command '/heatmap':

**Prototype**: FBA.CLI::Int32 Heatmap(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Draw heatmap from the correlations between the genes and the metabolism flux.
  Usage:        G:\GCModeller\manual\bin\FBA.exe /heatmap /x <matrix.csv> [/out <out.tiff> /name <Name> /width <8000> /height <6000>]
  Example:      FBA /heatmap 
```

##### Help for command '/heatmap.scale':

**Prototype**: FBA.CLI::Int32 ScaleHeatmap(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe /heatmap.scale /x <matrix.csv> [/factor 30 /out <out.csv>]
  Example:      FBA /heatmap.scale 
```

##### Help for command '/Imports':

**Prototype**: FBA.CLI::Int32 ImportsRxns(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe /Imports /in <sbml.xml>
  Example:      FBA /Imports 
```

##### Help for command '/phenos.MAT':

**Prototype**: FBA.CLI::Int32 ObjMAT(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Merges the objective function result as a Matrix. For calculation the coefficient of the genes with the phenotype objective function.
  Usage:        G:\GCModeller\manual\bin\FBA.exe /phenos.MAT /in <inDIR> [/out <outcsv>]
  Example:      FBA /phenos.MAT 
```

##### Help for command '/phenos.out.Coefficient':

**Prototype**: FBA.CLI::Int32 PhenosOUTCoefficient(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  2. Coefficient of the genes with the metabolism fluxs from the batch analysis result.
  Usage:        G:\GCModeller\manual\bin\FBA.exe /phenos.out.Coefficient /gene <samplesCopy.RPKM.csv> /pheno <samples.phenos_out.csv> [/footprints <footprints.csv> /out <out.csv> /spcc]
  Example:      FBA /phenos.out.Coefficient 
```

##### Help for command '/phenos.out.MAT':

**Prototype**: FBA.CLI::Int32 PhenoOUT_MAT(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  1. Merge flux.csv result as a Matrix, for the calculation of the coefficient of the genes with the metabolism flux.
  Usage:        G:\GCModeller\manual\bin\FBA.exe /phenos.out.MAT /in <inDIR> /samples <sampleTable.csv> [/out <outcsv> /model <MetaCyc.sbml>]
  Example:      FBA /phenos.out.MAT 
```

##### Help for command '/Solve':

**Prototype**: FBA.CLI::Int32 Solve(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  solve a FBA model from a specific (SBML) model file.
  Usage:        G:\GCModeller\manual\bin\FBA.exe /solve -i <sbml_file> -o <output_result_dir> -d <max/min> [-m <sbml/model> -f <object_function> -knock_out <gene_id_list>]
  Example:      FBA /Solve solve -i "/home/xieguigang/BLAST/db/MetaCyc/xcc8004/fba.xml" -o "/home/xieguigang/Desktop/8004" -m sbml -f default -d max -knock_out XC_1184,XC_3631
```



  Parameters information:
```
    -i
    Description:  

    Example:      -i ""

-o
    Description:  The directory for the output result.

    Example:      -o "/home/xieguigang/Desktop/8004"

   [-m]
    Description:  

    Example:      -m ""

   [-f]
    Description:  Optional, Set up the objective function for the fba linear programming problem, its value can be a expression, default or all.
 <expression> - a user specific expression for objective function, it can be a expression or a text file name if the first character is @ in the switch value.
 default - the program generate the objective function using the objective coefficient value which defines in each reaction object;
 all - set up all of the reaction objective coeffecient factor to 1, which means all of the reaction flux will use for objective function generation.

    Example:      -f "@d:/fba_objf.txt"

   [-d]
    Description:  Optional, the constraint direction of the objective function for the fba linear programming problem, 
if this switch option is not specific by the user then the program will use the direction which was defined in the FBA model file 
else if use specific this switch value then the user specific value will override the direction value in the FBA model.

    Example:      -d "max"

   [-knock_out]
    Description:  Optional, this switch specific the id list that of the gene will be knock out in the simulation, this switch option only works in the advanced fba model file.
value string format: each id can be seperated by the comma character and the id value can be both of the genbank id or a metacyc unique-id value.

    Example:      -knock_out "XC_1184,XC_3631"


```

#### Accepted Types
##### -i
##### -o
##### -m
##### -f
##### -d
##### -knock_out
##### Help for command '/Solver.KEGG':

**Prototype**: FBA.CLI::Int32 KEGGSolver(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe /Solver.KEGG /in <model.xml> /objs <locus.txt> [/out <outDIR>]
  Example:      FBA /Solver.KEGG 
```



  Parameters information:
```
    /objs
    Description:  This parameter defines the objective function in the FBA solver, is a text file which contains a list of genes locus, 
                   and these genes locus is associated to a enzyme reaction in the FBA model.

    Example:      /objs ""


```

#### Accepted Types
##### /objs
##### Help for command '/Solver.rFBA':

**Prototype**: FBA.CLI::Int32 AnalysisPhenotype(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\FBA.exe /Solver.rFBA /in <MetaCyc.Sbml> /reg <footprints.csv> /obj <object_function.txt/xml> [/obj-type <lst/pathway/module> /params <rfba.parameters.xml> /stat <stat.Csv> /sample <sampleName> /modify <locus_modify.csv> /out <outDIR>]
  Example:      FBA /Solver.rFBA 
```



  Parameters information:
```
       [/obj-type]
    Description:  The input document type of the objective function, default is a gene_locus list in a text file, alternative format can be KEGG pathway xml and KEGG module xml.

    Example:      /obj-type ""


```

#### Accepted Types
##### /obj-type
##### Help for command 'compile':

**Prototype**: FBA.CLI::Int32 Compile(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.
  Usage:        G:\GCModeller\manual\bin\FBA.exe compile -i <input_file> -o <output_file>[ -if <sbml/metacyc> -of <fba/fba2> -f <objective_function> -d <max/min>]
  Example:      FBA compile compile -i /home/xieguigang/ecoli/ -o /home/xieguigang/ecoli.xml -if metacyc -of fba2 -f v2+v3 -d max
```



  Parameters information:
```
    -i
    Description:  The input datasource path of the compiled model, it can be a MetaCyc data directory or a xml file in sbml format, format was specific by the value of switch '-if'

    Example:      -i "/home/xieguigang/ecoli/"

-o
    Description:  The output file path of the compiled model file.

    Example:      -o "/home/xieguigang/ecoli.xml"

   [-if]
    Description:  Optional, this switch specific the format of the input data source, the fba compiler just support the metacyc database and sbml model currently, default value if metacyc.
 metacyc - the input compiled data source is a metacyc database;
sbml - the input compiled data source is a standard sbml language model in level 2.

    Example:      -if "metacyc"

   [-of]
    Description:  Optional, this switch specific the format of the output compiled model, it can be a standard fba model or a advanced version of fba model, defualt is a standard fba model.
 fba - the output compiled model is a standard fba model;
fba2 - the output compiled model is a advanced version of fba model.

    Example:      -of "fba2"

   [-f]
    Description:  Optional, you can specific the objective function using this switch, default value is the objective function that define in the sbml model file.

    Example:      -f "v2+v3"

   [-d]
    Description:  Optional, the constraint direction of the objective function in the fba model, default value is maximum the objective function.
 max - the constraint direction is maximum;
 min - the constraint direction is minimum.

    Example:      -d "max"


```

#### Accepted Types
##### -i
##### -o
##### -if
##### -of
##### -f
##### -d
