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
+ [filter](OTU_table/filter.1) filter the otu data which has relative abundance greater than the given threshold
