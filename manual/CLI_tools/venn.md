---
title: venn
tags: [maunal, tools]
date: 7/7/2016 6:52:14 PM
---
# GCModeller [version 1.3.11.2]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/venn.exe
**Root namespace**: LANS.SystemsBiology.AnalysisTools.DataVisualization.VennDiagramTools.CLI


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|.Draw|Draw the venn diagram from a csv data file, you can specific the diagram drawing options from this command switch value. The generated venn dragram will be saved as tiff file format.|

## Commands
--------------------------
##### Help for command '.Draw':

**Prototype**: LANS.SystemsBiology.AnalysisTools.DataVisualization.VennDiagramTools.CLI::Int32 VennDiagramA(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  Draw the venn diagram from a csv data file, you can specific the diagram drawing options from this command switch value. The generated venn dragram will be saved as tiff file format.
  Usage:        G:\GCModeller\manual\bin\venn.exe .Draw -i <csv_file> [-t <diagram_title> -o <_diagram_saved_path> -s <partitions_option_pairs> -rbin <r_bin_directory>]
  Example:      venn .Draw .Draw -i /home/xieguigang/Desktop/genomes.csv -t genome-compared -o ~/Desktop/xcc8004.tiff -s "Xcc8004,blue,Xcc 8004;ecoli,green,Ecoli. K12;pa14,yellow,PA14;ftn,black,FTN;aciad,red,ACIAD"
```



  Parameters information:
```
    -i
    Description:  The csv data source file for drawing the venn diagram graph.

    Example:      -i "/home/xieguigang/Desktop/genomes.csv"

   [-t]
    Description:  Optional, the venn diagram title text

    Example:      -t "genome-compared"

   [-o]
    Description:  Optional, the saved file location for the venn diagram, if this switch value is not specific by the user then 
              the program will save the generated venn diagram to user desktop folder and using the file name of the input csv file as default.

    Example:      -o "~/Desktop/xcc8004.tiff"

   [-s]
    Description:  Optional, the profile settings for the partitions in the venn diagram, each partition profile data is
               in a key value paired like: name,color, and each partition profile pair is seperated by a ';' character.
              If this switch value is not specific by the user then the program will trying to parse the partition name
              from the column values and apply for each partition a randomize color.

    Example:      -s "Xcc8004,blue,Xcc 8004;ecoli,green,Ecoli. K12;pa14,yellow,PA14;ftn,black,FTN;aciad,red,ACIAD"

   [-rbin]
    Description:  Optional, Set up the r bin path for drawing the venn diagram, if this switch value is not specific by the user then 
              the program just output the venn diagram drawing R script file in a specific location, or if this switch 
              value is specific by the user and is valid for call the R program then will output both venn diagram tiff image file and R script for drawing the output venn diagram.
              This switch value is just for the windows user, when this program was running on a LINUX/UNIX/MAC platform operating 
              system, you can ignore this switch value, but you should install the R program in your linux/MAC first if you wish to
               get the venn diagram directly from this program.

    Example:      -rbin "C:\\R\\bin\\"


```

#### Accepted Types
##### -i
##### -t
##### -o
##### -s
##### -rbin
