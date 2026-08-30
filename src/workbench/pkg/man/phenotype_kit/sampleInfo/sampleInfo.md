# sampleInfo

GCModeller DEG experiment analysis designer toolkit
> This R# package module provides the toolkit for create and manipulate the 
>  experiment sample information data(@``T:SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner.SampleInfo``), which is the 
>  experiment design data of the different expression analysis:
>  
>  + create the sample information data: ``sampleInfo``, 
>    ``guess.sample_groups``, ``sampleinfo.text.groups``, ``read.sampleinfo``;
>  + manipulate the sample group data: ``design``, ``sample_groups``, 
>    ``shuffle_groups``, ``group.colors``, ``sampleinfo_gsub``, ``sampleId``;
>  + build the analysis model for run the different expression analysis: 
>    ``make.analysis``, ``make.MLdataset``.
>  
>  The sample information data object in R# environment can be saved as a csv 
>  table file via the ``write.sampleinfo`` api, or be converted to a data frame 
>  via the ``as.data.frame`` api.

+ [guess.sample_groups](sampleInfo/guess.sample_groups.1) try to parse the sampleInfo data from the
+ [group.colors](sampleInfo/group.colors.1) get/set the group colors
+ [design](sampleInfo/design.1) Create new analysis design sample info via formula
+ [read.sampleinfo](sampleInfo/read.sampleinfo.1) Read the sampleinfo data table from a given csv file
+ [shuffle_groups](sampleInfo/shuffle_groups.1) shuffle the sample group order in a random manner
+ [sample_groups](sampleInfo/sample_groups.1) group the sample information data by the sample group label
+ [write.sampleinfo](sampleInfo/write.sampleinfo.1) save sampleinfo data as csv file
+ [sampleInfo](sampleInfo/sampleInfo.1) create ``sample_info`` data table
+ [sampleinfo_gsub](sampleInfo/sampleinfo_gsub.1) do text replace of the sample group label
+ [sampleId](sampleInfo/sampleId.1) Get sample id collection from a speicifc sample data groups
+ [sampleinfo.text.groups](sampleInfo/sampleinfo.text.groups.1) Create sampleInfo table from text files
+ [make.analysis](sampleInfo/make.analysis.1) create the different expression analysis design of the control vs treatment
+ [make.MLdataset](sampleInfo/make.MLdataset.1) create the machine learning dataset from the gene expression matrix and the 
