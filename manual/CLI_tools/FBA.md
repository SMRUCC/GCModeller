---
title: FBA
tags: [maunal, tools]
date: 2016/10/22 12:30:09
---
# GCModeller [version 1.1.24.3]
> 

<!--more-->

**FBA(Flux Balance Analysis) Model Solver**
_FBA(Flux Balance Analysis) Model Solver_
Copyright ? xie.guigang@gmail.com. 2013

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/FBA.exe
**Root namespace**: ``FBA.CLI``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[/Analysis.Phenotype](#/Analysis.Phenotype)||
|[/Export](#/Export)||
|[/Flux.Coefficient](#/Flux.Coefficient)||
|[/Flux.KEGG.Filter](#/Flux.KEGG.Filter)||
|[/Func.Coefficient](#/Func.Coefficient)||
|[/gcFBA.Batch](#/gcFBA.Batch)||
|[/heatmap](#/heatmap)|Draw heatmap from the correlations between the genes and the metabolism flux.|
|[/heatmap.scale](#/heatmap.scale)||
|[/Imports](#/Imports)||
|[/phenos.MAT](#/phenos.MAT)|Merges the objective function result as a Matrix. For calculation the coefficient of the genes with the phenotype objective function.|
|[/phenos.out.Coefficient](#/phenos.out.Coefficient)|2. Coefficient of the genes with the metabolism fluxs from the batch analysis result.|
|[/phenos.out.MAT](#/phenos.out.MAT)|1. Merge flux.csv result as a Matrix, for the calculation of the coefficient of the genes with the metabolism flux.|
|[/Solve](#/Solve)|solve a FBA model from a specific (SBML) model file.|
|[/Solver.KEGG](#/Solver.KEGG)||
|[/Solver.rFBA](#/Solver.rFBA)||
|[compile](#compile)|Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.|

## CLI API list
--------------------------
<h3 id="/Analysis.Phenotype"> 1. /Analysis.Phenotype</h3>


**Prototype**: ``FBA.CLI::Int32 rFBABatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /Analysis.Phenotype /in <MetaCyc.Sbml> /reg <footprints.csv> /obj <list/path/module-xml> [/obj-type <lst/pathway/module> /params <rfba.parameters.xml> /stat <stat.Csv> /sample <sampleTable.csv> /modify <locus_modify.csv> /out <outDIR>]
```
<h3 id="/Export"> 2. /Export</h3>


**Prototype**: ``FBA.CLI::Int32 Export(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA export -i <fba_model> -o <r_script>
```
###### Example
```bash
FBA export -i /home/xieguigang/ecoli.xml -o /home/xieguigang/ecoli.r
```
<h3 id="/Flux.Coefficient"> 3. /Flux.Coefficient</h3>


**Prototype**: ``FBA.CLI::Int32 FluxCoefficient(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /Flux.Coefficient /in <rFBA.result_dumpDIR> [/footprints <footprints.csv> /out <outCsv> /spcc /KEGG]
```
<h3 id="/Flux.KEGG.Filter"> 4. /Flux.KEGG.Filter</h3>


**Prototype**: ``FBA.CLI::Int32 KEGGFilter(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /Flux.KEGG.Filter /in <flux.csv> /model <MetaCyc.sbml> [/out <out.csv>]
```
<h3 id="/Func.Coefficient"> 5. /Func.Coefficient</h3>


**Prototype**: ``FBA.CLI::Int32 FuncCoefficient(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /Func.Coefficient /func <objfunc_matrix.csv> /in <rFBA.result_dumpDIR> [/footprints <footprints.csv> /out <outCsv> /spcc]
```
<h3 id="/gcFBA.Batch"> 6. /gcFBA.Batch</h3>


**Prototype**: ``FBA.CLI::Int32 PhenotypeAnalysisBatch(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /gcFBA.Batch /model <model.sbml> /phenotypes <KEGG_modules/pathways.DIR> /footprints <footprints.csv> [/obj-type <pathway/module> /params <rfba.parameters.xml> /stat <RPKM-stat.Csv> /sample <sampleTable.csv> /modify <locus_modify.csv> /out <outDIR> /parallel <2>]
```
<h3 id="/heatmap"> 7. /heatmap</h3>

Draw heatmap from the correlations between the genes and the metabolism flux.
**Prototype**: ``FBA.CLI::Int32 Heatmap(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /heatmap /x <matrix.csv> [/out <out.tiff> /name <Name> /width <8000> /height <6000>]
```
<h3 id="/heatmap.scale"> 8. /heatmap.scale</h3>


**Prototype**: ``FBA.CLI::Int32 ScaleHeatmap(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /heatmap.scale /x <matrix.csv> [/factor 30 /out <out.csv>]
```
<h3 id="/Imports"> 9. /Imports</h3>


**Prototype**: ``FBA.CLI::Int32 ImportsRxns(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /Imports /in <sbml.xml>
```
<h3 id="/phenos.MAT"> 10. /phenos.MAT</h3>

Merges the objective function result as a Matrix. For calculation the coefficient of the genes with the phenotype objective function.
**Prototype**: ``FBA.CLI::Int32 ObjMAT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /phenos.MAT /in <inDIR> [/out <outcsv>]
```
<h3 id="/phenos.out.Coefficient"> 11. /phenos.out.Coefficient</h3>

2. Coefficient of the genes with the metabolism fluxs from the batch analysis result.
**Prototype**: ``FBA.CLI::Int32 PhenosOUTCoefficient(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /phenos.out.Coefficient /gene <samplesCopy.RPKM.csv> /pheno <samples.phenos_out.csv> [/footprints <footprints.csv> /out <out.csv> /spcc]
```
<h3 id="/phenos.out.MAT"> 12. /phenos.out.MAT</h3>

1. Merge flux.csv result as a Matrix, for the calculation of the coefficient of the genes with the metabolism flux.
**Prototype**: ``FBA.CLI::Int32 PhenoOUT_MAT(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /phenos.out.MAT /in <inDIR> /samples <sampleTable.csv> [/out <outcsv> /model <MetaCyc.sbml>]
```
<h3 id="/Solve"> 13. /Solve</h3>

solve a FBA model from a specific (SBML) model file.
**Prototype**: ``FBA.CLI::Int32 Solve(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /solve -i <sbml_file> -o <output_result_dir> -d <max/min> [-m <sbml/model> -f <object_function> -knock_out <gene_id_list>]
```
###### Example
```bash
FBA solve -i "/home/xieguigang/BLAST/db/MetaCyc/xcc8004/fba.xml" -o "/home/xieguigang/Desktop/8004" -m sbml -f default -d max -knock_out XC_1184,XC_3631
```


#### Arguments
##### -i


###### Example
```bash
-i <term_string>
```
##### -o
The directory for the output result.

###### Example
```bash
-o /home/xieguigang/Desktop/8004
```
##### [-m]


###### Example
```bash
-m <term_string>
```
##### [-f]
Optional, Set up the objective function for the fba linear programming problem, its value can be a expression, default or all.
<expression> - a user specific expression for objective function, it can be a expression or a text file name if the first character is @ in the switch value.
default - the program generate the objective function using the objective coefficient value which defines in each reaction object;
all - set up all of the reaction objective coeffecient factor to 1, which means all of the reaction flux will use for objective function generation.

###### Example
```bash
-f @d:/fba_objf.txt
```
##### [-d]
Optional, the constraint direction of the objective function for the fba linear programming problem,
if this switch option is not specific by the user then the program will use the direction which was defined in the FBA model file
else if use specific this switch value then the user specific value will override the direction value in the FBA model.

###### Example
```bash
-d max
```
##### [-knock_out]
Optional, this switch specific the id list that of the gene will be knock out in the simulation, this switch option only works in the advanced fba model file.
value string format: each id can be seperated by the comma character and the id value can be both of the genbank id or a metacyc unique-id value.

###### Example
```bash
-knock_out XC_1184,XC_3631
```
<h3 id="/Solver.KEGG"> 14. /Solver.KEGG</h3>


**Prototype**: ``FBA.CLI::Int32 KEGGSolver(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /Solver.KEGG /in <model.xml> /objs <locus.txt> [/out <outDIR>]
```


#### Arguments
##### /objs
This parameter defines the objective function in the FBA solver, is a text file which contains a list of genes locus,
and these genes locus is associated to a enzyme reaction in the FBA model.

###### Example
```bash
/objs <term_string>
```
<h3 id="/Solver.rFBA"> 15. /Solver.rFBA</h3>


**Prototype**: ``FBA.CLI::Int32 AnalysisPhenotype(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA /Solver.rFBA /in <MetaCyc.Sbml> /reg <footprints.csv> /obj <object_function.txt/xml> [/obj-type <lst/pathway/module> /params <rfba.parameters.xml> /stat <stat.Csv> /sample <sampleName> /modify <locus_modify.csv> /out <outDIR>]
```


#### Arguments
##### [/obj-type]
The input document type of the objective function, default is a gene_locus list in a text file, alternative format can be KEGG pathway xml and KEGG module xml.

###### Example
```bash
/obj-type <term_string>
```
<h3 id="compile"> 16. compile</h3>

Compile data source into a model file so that the fba program can using the data to performing the simulation calculation.
**Prototype**: ``FBA.CLI::Int32 Compile(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
FBA compile -i <input_file> -o <output_file>[ -if <sbml/metacyc> -of <fba/fba2> -f <objective_function> -d <max/min>]
```
###### Example
```bash
FBA compile -i /home/xieguigang/ecoli/ -o /home/xieguigang/ecoli.xml -if metacyc -of fba2 -f v2+v3 -d max
```


#### Arguments
##### -i
The input datasource path of the compiled model, it can be a MetaCyc data directory or a xml file in sbml format, format was specific by the value of switch '-if'

###### Example
```bash
-i /home/xieguigang/ecoli/
```
##### -o
The output file path of the compiled model file.

###### Example
```bash
-o /home/xieguigang/ecoli.xml
```
##### [-if]
Optional, this switch specific the format of the input data source, the fba compiler just support the metacyc database and sbml model currently, default value if metacyc.
metacyc - the input compiled data source is a metacyc database;
sbml - the input compiled data source is a standard sbml language model in level 2.

###### Example
```bash
-if metacyc
```
##### [-of]
Optional, this switch specific the format of the output compiled model, it can be a standard fba model or a advanced version of fba model, defualt is a standard fba model.
fba - the output compiled model is a standard fba model;
fba2 - the output compiled model is a advanced version of fba model.

###### Example
```bash
-of fba2
```
##### [-f]
Optional, you can specific the objective function using this switch, default value is the objective function that define in the sbml model file.

###### Example
```bash
-f v2+v3
```
##### [-d]
Optional, the constraint direction of the objective function in the fba model, default value is maximum the objective function.
max - the constraint direction is maximum;
min - the constraint direction is minimum.

###### Example
```bash
-d max
```
