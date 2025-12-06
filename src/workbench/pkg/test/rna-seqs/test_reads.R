require(GCModeller);

imports "FastQ" from "rnaseq";
imports ["bioseq.fasta","GenBank"] from "seqtoolkit";

let asm_source = GenBank::load_genbanks(list.files("D:\\datapool\\ncbi_genbank") |> take(100));
let genomes = sapply(asm_source, gb -> GenBank::origin_fasta(gb));
let names = [genomes]::Title;
let w1 = list(
    "CP000946 Escherichia coli ATCC 8739, complete genome. 4.75 MB" = 33,
    "CP001637 Escherichia coli DH1, complete genome. 4.63 MB" = 500,
    "CP002168 Escherichia coli UM146 plasmid pUM146, complete sequence. 114.55 KB" = 699
);
let w2 = list(
    "AP014857 Escherichia albertii DNA, complete genome, strain: EC06-170. 4.66 MB" = 2000,
);

# print(names, max.print = 10000);

for(i in 1:9) {
    let reads1 = FastQ::simulate_reads(genomes, n= 100000, genome_weights = w1);
    let reads2 = FastQ::simulate_reads(genomes, n= 100000, genome_weights = w2);
    let save1 = sprintf("Z:/w1_%s.fq", i);
    let save2 = sprintf("Z:/w2_%s.fq", i);

    write.fastq(reads1, file = save1);
    write.fastq(reads2, file = save2);
}

