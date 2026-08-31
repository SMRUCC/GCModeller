# miRNA

miRNA/siRNA target gene prediction toolkit
> This R# package module provides the toolkit for predict the target genes of 
>  the miRNA/siRNA small RNA sequence:
>  
>  + ``psRNATarget`` and ``TargetFinder``: create the miRNA target site match 
>    algorithm object;
>  + ``miRNA_targets``: run the target site match of the given miRNA sequence 
>    against the candidate target mRNA/CDS sequence collection;
>  + ``intersect_targets``: take the intersection of the two algorithm result for 
>    create the high confidence target site set.
>  
>  the generated match result is a collection of the @``T:SMRUCC.genomics.Analysis.SequenceAlignment.siRNAHit.siRNAHit`` 
>  object, which can be converted to a data frame via the ``as.data.frame`` api, 
>  or be saved as a csv table file via the ``write.csv`` api.

+ [mirna_blastn](miRNA/mirna_blastn.1) 
+ [parse_blastn](miRNA/parse_blastn.1) 
+ [blastn_filter](miRNA/blastn_filter.1) 
+ [psRNATarget](miRNA/psRNATarget.1) create the psRNATarget algorithm object for predict the miRNA target site
+ [TargetFinder](miRNA/TargetFinder.1) create the TargetFinder algorithm object for predict the miRNA target site
+ [miRNA_targets](miRNA/miRNA_targets.1) make matches of the miRNA target genes
+ [intersect_targets](miRNA/intersect_targets.1) --- High-confidence intersection (psRNATarget ∩ TargetFinder) ---
