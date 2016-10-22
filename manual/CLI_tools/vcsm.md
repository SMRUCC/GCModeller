---
title: vcsm
tags: [maunal, tools]
date: 2016/10/22 12:30:19
---
# virtualcell simulations host [version 2.6.0.255]
> 

<!--more-->

**GCModeller biosystem simulation console & modelling engine host**
_virtualcell simulations host_
Copyright ? Lans Corp. 2013

**Module AssemblyName**: file:///E:/GCModeller/GCModeller/bin/vcsm.exe
**Root namespace**: ``LANS.SystemsBiology.GCModeller.CommandLines``


All of the command that available in this program has been list below:

##### Generic function API list
|Function API|Info|
|------------|----|
|[Experiment.Whole_Genome_Mutation](#Experiment.Whole_Genome_Mutation)|shell parameter is the shoal shell application program file location.|
|[Experiment.Whole_Genome_Mutation2](#Experiment.Whole_Genome_Mutation2)|shell parameter is the shoal shell application program file location. this command is required the GCML format model file.|
|[load.model.csv_tabular](#load.model.csv_tabular)|The csv_tabular format model file is the alternative format of the GCModeller virtual cell modle, as the GCModeller only support the GCML xml file as the modelling data source, so that you should using this command to load the csv_tabular format model file as the GCML format.|
|[registry](#registry)||
|[run](#run)||
|[unregistry](#unregistry)||

## CLI API list
--------------------------
<h3 id="Experiment.Whole_Genome_Mutation"> 1. Experiment.Whole_Genome_Mutation</h3>

shell parameter is the shoal shell application program file location.
**Prototype**: ``LANS.SystemsBiology.GCModeller.CommandLines::Int32 WholeGenomeMutation(System.String, Double, System.String)``

###### Usage
```bash
vcsm
```
<h3 id="Experiment.Whole_Genome_Mutation2"> 2. Experiment.Whole_Genome_Mutation2</h3>

shell parameter is the shoal shell application program file location. this command is required the GCML format model file.
**Prototype**: ``LANS.SystemsBiology.GCModeller.CommandLines::Int32 WholeGenomeMutationFromGCML(System.String, Double, System.String)``

###### Usage
```bash
vcsm
```
<h3 id="load.model.csv_tabular"> 3. load.model.csv_tabular</h3>

The csv_tabular format model file is the alternative format of the GCModeller virtual cell modle, as the GCModeller only support the GCML xml file as the modelling data source, so that you should using this command to load the csv_tabular format model file as the GCML format.
**Prototype**: ``LANS.SystemsBiology.GCModeller.CommandLines::SMRUCC.genomics.GCModeller.ModellingEngine.EngineSystem.Engine.GCModeller LoadCsv(System.String, Microsoft.VisualBasic.Logging.LogFile, Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
vcsm
```
<h3 id="registry"> 4. registry</h3>


**Prototype**: ``LANS.SystemsBiology.GCModeller.CommandLines::Int32 RegistryModule(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
vcsm registry <assembly_file>
```
###### Example
```bash
vcsm resistry /home/xieguigang/gcmodeller/models/plas.dll
```
<h3 id="run"> 5. run</h3>


**Prototype**: ``LANS.SystemsBiology.GCModeller.CommandLines::Int32 Run(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
vcsm run -i <model_file> -mysql <mysql_connection_string> [-f <gcml/csv_tabular> -t <time> -metabolism <assembly_path> -expression <assembly_path>]
```
###### Example
```bash
vcsm run -i ~/gc/ecoli.xml -t 1000 -url "http://localhost:8080/client?user=username%password=password%database=database"
```


#### Arguments
##### -i
This switch value specific the model file that the simulation engine will be load

###### Example
```bash
-i ~/gc/ecoli.xml
```
##### -url
Setup the data storage service connection url string.

###### Example
```bash
-url http://localhost:8080/client?user=username%password=password%database=database
```
##### [-t]
Optional, This switch specific that the cycle number of this simulation will run, this switch value will override the time value in the loaded model file.

###### Example
```bash
-t 1000
```
##### [-metabolism]
N/A - The engine kernel will not load the metabolism module.

###### Example
```bash
-metabolism <term_string>
```
##### [-expression]
N/A - The engine kernel will not load the gene expression regulation module.

###### Example
```bash
-expression <term_string>
```
##### [-interval]
This switch value specific the data commit to the mysql database server time interval, if your compiled model is too large you should consider set up this switch value smaller in order to avoid the unexpected memory out of range exception.

###### Example
```bash
-interval <term_string>
```
##### [-f]
This parameter specific the file format of the target input model file, default value is gcml format.

###### Example
```bash
-f <term_string>
```
##### [-suppress_warn]
T/TRUE/F/FALSE

###### Example
```bash
-suppress_warn <term_string>
```
##### [-suppress_error]
T/TRUE/F/FALSE

###### Example
```bash
-suppress_error <term_string>
```
##### [-suppress_periodic_message]
T/TRUE/F/FALSE

###### Example
```bash
-suppress_periodic_message <term_string>
```
<h3 id="unregistry"> 6. unregistry</h3>


**Prototype**: ``LANS.SystemsBiology.GCModeller.CommandLines::Int32 UnRegistry(args As Microsoft.VisualBasic.CommandLine.CommandLine)``

###### Usage
```bash
vcsm unregistry <assembly_file>
```
###### Example
```bash
vcsm unregistry ~/gcmodeller/models/plas.dll
```
