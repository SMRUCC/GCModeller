# bifrost

Bifrost: the gene prediction toolkit
> This R# package module provides the api for run gene prediction on the 
>  genomics contigs assembly sequence:
>  
>  + ``prodigal``: the ab-initio prokaryotic gene prediction algorithm 
>    (PROkaryotic DYnamic programming Gene-finding ALgorithm), works on the 
>    prokaryotic MAGs contigs assembly sequence;
>  + ``metaeuk``: the homology based eukaryotic gene prediction algorithm, 
>    works on the eukaryotic contigs assembly sequence with a given reference 
>    protein database;
>    
>  The gene prediction result of the prodigal algorithm is a collection of 
>  the ``PredictionResult`` object, which could be exported as:
>  
>  + GFF3 table via the ``as.gff3`` api;
>  + nucleotide/protein fasta sequence via the ``as.genes``/``as.proteins`` api;
>  + a score table data frame via the ``as.data.frame`` api, for save as a csv 
>    file by the ``write.csv`` api.

+ [prodigal_training](bifrost/prodigal_training.1) Train the gene prediction model in an unsupervised manner
+ [prodigal](bifrost/prodigal.1) Prodigal (PROkaryotic DYnamic programming Gene-finding ALgorithm)
+ [metaeuk](bifrost/metaeuk.1) MetaEuk: the homology based eukaryotic gene prediction
+ [as.gff3](bifrost/as.gff3.1) cast the gene prediction result as GFF3 table format
+ [as.proteins](bifrost/as.proteins.1) Extract the protein sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
+ [as.genes](bifrost/as.genes.1) Extract the gene sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
