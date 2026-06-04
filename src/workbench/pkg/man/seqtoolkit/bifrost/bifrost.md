# bifrost



+ [as.data.frame](bifrost/as.data.frame.1) 
+ [prodigal_training](bifrost/prodigal_training.1) 
+ [prodigal](bifrost/prodigal.1) Prodigal (PROkaryotic DYnamic programming Gene-finding ALgorithm)
+ [as.gff3](bifrost/as.gff3.1) cast the gene prediction result as GFF3 table format
+ [as.proteins](bifrost/as.proteins.1) Extract the protein sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
+ [as.genes](bifrost/as.genes.1) Extract the gene sequences from the gene prediction result, and return as FASTA format. The sequence ID is in the format of "seqid_geneindex". For example, "contig1_5" means the 5th predicted gene on contig1. The sequence description is in the format of "start-end(strand)". For example, "100-900(+)" means the gene starts at position 100, ends at position 900, and is on the forward strand.
