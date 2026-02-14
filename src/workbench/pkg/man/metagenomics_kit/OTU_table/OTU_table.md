# OTU_table

Tools for handling OTU table data
> ### Operational taxonomic unit (OTU)
>  
>  OTU's are used to categorize bacteria based on sequence similarity.
>  
>  In 16S metagenomics approaches, OTUs are cluster of similar sequence variants of the 
>  16S rDNA marker gene sequence. Each of these cluster is intended to represent a 
>  taxonomic unit of a bacteria species or genus depending on the sequence similarity 
>  threshold. Typically, OTU cluster are defined by a 97% identity threshold of the 16S 
>  gene sequences to distinguish bacteria at the genus level.
> 
>  Species separation requires a higher threshold Of 98% Or 99% sequence identity, Or 
>  even better the use Of exact amplicon sequence variants (ASV) instead Of OTU sequence 
>  clusters.

+ [relative_abundance](OTU_table/relative_abundance.1) Transform abundance data in an otu_table to relative abundance, sample-by-sample. 
+ [median_scale](OTU_table/median_scale.1) 
+ [impute_missing](OTU_table/impute_missing.1) 
+ [filter](OTU_table/filter.1) filter the otu data which has relative abundance greater than the given threshold
+ [read.OTUtable](OTU_table/read.OTUtable.1) read 16s OTU table
+ [read.OTUdata](OTU_table/read.OTUdata.1) 
+ [batch_combine](OTU_table/batch_combine.1) combine of two batch data
+ [otu_from_matrix](OTU_table/otu_from_matrix.1) cast the expression matrix to the otu data
+ [as.hts_matrix](OTU_table/as.hts_matrix.1) Create expression matrix data from a given otu table
+ [as.OTU_table](OTU_table/as.OTU_table.1) convert the mothur rank tree as the OTU table
+ [make_otu_table](OTU_table/make_otu_table.1) 
+ [makeTreeGraph](OTU_table/makeTreeGraph.1) make OTU tree graph via JSD correlation method
+ [makeUPGMATree](OTU_table/makeUPGMATree.1) 
