require(GCModeller);

imports "FastQ" from "rnaseq";
imports ["bioseq.fasta","GenBank"] from "seqtoolkit";

let asm_source = GenBank::load_genbanks(list.files("D:\\datapool\\ncbi_genbank") |> take(100));
let genomes = lapply(asm_source, gb -> GenBank::origin_fasta(gb));
let reads_data = FastQ::simulate_reads(genomes, n= 1000);

write.fastq(reads_data, file = "Z:/test.fq");