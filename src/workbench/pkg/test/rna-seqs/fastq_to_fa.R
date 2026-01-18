require(GCModeller);

imports "FastQ" from "rnaseq";
imports "bioseq.fasta" from "seqtoolkit";

let reads = read.fastq("U:\metagenomics_LLMs\demo\meta_pipeline_package\meta_pipeline_package\new_test\processed_reads\sample01\filtered_fq\nonhost.fq");
let seqs = as.fasta(reads);

write.fasta(seqs, file = "Z:/test_reads.fasta");