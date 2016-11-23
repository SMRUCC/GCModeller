---
title: venn
tags: [maunal, tools]
date: 11/24/2016 2:54:30 AM
---
# GCModeller [version 1.3.11.2]
> Tools for creating venn diagram model for the R program and venn diagram visualize drawing.

<!--more-->

**Venn Diagram Data Visualization**<br/>
_Venn Diagram Data Visualization_<br/>
Copyright © LANS Engineering Workstation 2013

**Module AssemblyName**: file:///G:/GCModeller/GCModeller/bin/venn.exe<br/>
**Root namespace**: ``LANS.SystemsBiology.AnalysisTools.DataVisualization.VennDiagramTools.CLI``<br/>


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[.Draw](#.Draw)|Draw the venn diagram from a csv data file, you can specific the diagram drawing options from this command switch value. The generated venn dragram will be saved as tiff file format.|

## CLI API list
--------------------------
<h3 id=".Draw"> 1. .Draw</h3>

Draw the venn diagram from a csv data file, you can specific the diagram drawing options from this command switch value. The generated venn dragram will be saved as tiff file format.
**Prototype**: ``LANS.SystemsBiology.AnalysisTools.DataVisualization.VennDiagramTools.CLI::Int32 VennDiagramA(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
venn .Draw -i <csv_file> [-t <diagram_title> -o <_diagram_saved_path> -s <partitions_option_pairs> -rbin <r_bin_directory>]
```
###### Example
```bash
venn .Draw -i /home/xieguigang/Desktop/genomes.csv -t genome-compared -o ~/Desktop/xcc8004.tiff -s "Xcc8004,blue,Xcc 8004;ecoli,green,Ecoli. K12;pa14,yellow,PA14;ftn,black,FTN;aciad,red,ACIAD"
```


#### Arguments
##### -i
The csv data source file for drawing the venn diagram graph.

###### Example
```bash
-i /home/xieguigang/Desktop/genomes.csv
```
##### [-t]
Optional, the venn diagram title text

###### Example
```bash
-t genome-compared
```
##### [-o]
Optional, the saved file location for the venn diagram, if this switch value is not specific by the user then
the program will save the generated venn diagram to user desktop folder and using the file name of the input csv file as default.

###### Example
```bash
-o ~/Desktop/xcc8004.tiff
```
##### [-s]
Optional, the profile settings for the partitions in the venn diagram, each partition profile data is
in a key value paired like: name,color, and each partition profile pair is seperated by a ';' character.
If this switch value is not specific by the user then the program will trying to parse the partition name
from the column values and apply for each partition a randomize color.

###### Example
```bash
-s "Xcc8004,blue,Xcc 8004;ecoli,green,Ecoli. K12;pa14,yellow,PA14;ftn,black,FTN;aciad,red,ACIAD"
```
##### [-rbin]
Optional, Set up the r bin path for drawing the venn diagram, if this switch value is not specific by the user then
the program just output the venn diagram drawing R script file in a specific location, or if this switch
value is specific by the user and is valid for call the R program then will output both venn diagram tiff image file and R script for drawing the output venn diagram.
This switch value is just for the windows user, when this program was running on a LINUX/UNIX/MAC platform operating
system, you can ignore this switch value, but you should install the R program in your linux/MAC first if you wish to
get the venn diagram directly from this program.

###### Example
```bash
-rbin C:\\R\\bin\\
```
