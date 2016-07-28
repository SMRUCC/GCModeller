---
title: vcsm
tags: [maunal, tools]
date: 7/27/2016 6:40:29 PM
---
# virtualcell simulations host [version 2.6.0.255]
**Module AssemblyName**: file:///G:/GCModeller/manual/bin/vcsm.exe
**Root namespace**: LANS.SystemsBiology.GCModeller.CommandLines


All of the command that available in this program has been list below:

|Function API|Info|
|------------|----|
|Experiment.Whole_Genome_Mutation|shell parameter is the shoal shell application program file location.|
|Experiment.Whole_Genome_Mutation2|shell parameter is the shoal shell application program file location. this command is required the GCML format model file.|
|load.model.csv_tabular|The csv_tabular format model file is the alternative format of the GCModeller virtual cell modle, as the GCModeller only support the GCML xml file as the modelling data source, so that you should using this command to load the csv_tabular format model file as the GCML format.|
|registry||
|run||
|unregistry||

## Commands
--------------------------
##### Help for command 'Experiment.Whole_Genome_Mutation':

**Prototype**: LANS.SystemsBiology.GCModeller.CommandLines::Int32 WholeGenomeMutation(System.String, Double, System.String)

```
  Information:  shell parameter is the shoal shell application program file location.
  Usage:        G:\GCModeller\manual\bin\vcsm.exe 
  Example:      vcsm Experiment.Whole_Genome_Mutation 
```

##### Help for command 'Experiment.Whole_Genome_Mutation2':

**Prototype**: LANS.SystemsBiology.GCModeller.CommandLines::Int32 WholeGenomeMutationFromGCML(System.String, Double, System.String)

```
  Information:  shell parameter is the shoal shell application program file location. this command is required the GCML format model file.
  Usage:        G:\GCModeller\manual\bin\vcsm.exe 
  Example:      vcsm Experiment.Whole_Genome_Mutation2 
```

##### Help for command 'load.model.csv_tabular':

**Prototype**: LANS.SystemsBiology.GCModeller.CommandLines::SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller LoadCsv(System.String, Microsoft.VisualBasic.Logging.LogFile, Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  The csv_tabular format model file is the alternative format of the GCModeller virtual cell modle, as the GCModeller only support the GCML xml file as the modelling data source, so that you should using this command to load the csv_tabular format model file as the GCML format.
  Usage:        G:\GCModeller\manual\bin\vcsm.exe 
  Example:      vcsm load.model.csv_tabular 
```

##### Help for command 'registry':

**Prototype**: LANS.SystemsBiology.GCModeller.CommandLines::Int32 RegistryModule(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\vcsm.exe registry <assembly_file>
  Example:      vcsm registry resistry /home/xieguigang/gcmodeller/models/plas.dll
```

##### Help for command 'run':

**Prototype**: LANS.SystemsBiology.GCModeller.CommandLines::Int32 Run(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\vcsm.exe run -i <model_file> -mysql <mysql_connection_string> [-f <gcml/csv_tabular> -t <time> -metabolism <assembly_path> -expression <assembly_path>]
  Example:      vcsm run run -i ~/gc/ecoli.xml -t 1000 -url "http://localhost:8080/client?user=username%password=password%database=database"
```



  Parameters information:
```
    -i
    Description:  This switch value specific the model file that the simulation engine will be load

    Example:      -i "~/gc/ecoli.xml"

-url
    Description:  Setup the data storage service connection url string.

    Example:      -url "http://localhost:8080/client?user=username%password=password%database=database"

   [-t]
    Description:  Optional, This switch specific that the cycle number of this simulation will run, this switch value will override the time value in the loaded model file.

    Example:      -t "1000"

   [-metabolism]
    Description:  N/A - The engine kernel will not load the metabolism module.

    Example:      -metabolism ""

   [-expression]
    Description:  N/A - The engine kernel will not load the gene expression regulation module.

    Example:      -expression ""

   [-interval]
    Description:  This switch value specific the data commit to the mysql database server time interval, if your compiled model is too large you should consider set up this switch value smaller in order to avoid the unexpected memory out of range exception.

    Example:      -interval ""

   [-f]
    Description:  This parameter specific the file format of the target input model file, default value is gcml format.

    Example:      -f ""

   [-suppress_warn]
    Description:  T/TRUE/F/FALSE

    Example:      -suppress_warn ""

   [-suppress_error]
    Description:  T/TRUE/F/FALSE

    Example:      -suppress_error ""

   [-suppress_periodic_message]
    Description:  T/TRUE/F/FALSE

    Example:      -suppress_periodic_message ""


```

#### Accepted Types
##### -i
##### -url
##### -t
##### -metabolism
##### -expression
##### -interval
##### -f
##### -suppress_warn
##### -suppress_error
##### -suppress_periodic_message
##### Help for command 'unregistry':

**Prototype**: LANS.SystemsBiology.GCModeller.CommandLines::Int32 UnRegistry(Microsoft.VisualBasic.CommandLine.CommandLine)

```
  Information:  
  Usage:        G:\GCModeller\manual\bin\vcsm.exe unregistry <assembly_file>
  Example:      vcsm unregistry unregistry ~/gcmodeller/models/plas.dll
```

