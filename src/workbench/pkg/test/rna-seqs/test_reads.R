require(GCModeller);

imports "FastQ" from "rnaseq";
imports ["bioseq.fasta","GenBank"] from "seqtoolkit";

let asm_source = GenBank::load_genbanks(list.files("D:\\datapool\\pathgen") |> take(1000));
let genomes = sapply(asm_source, gb -> GenBank::origin_fasta(gb));
let names = [genomes]::Title;
let w1 = list(
#     "CP000946 Escherichia coli ATCC 8739, complete genome. 4.75 MB" = 33,
#     "CP001637 Escherichia coli DH1, complete genome. 4.63 MB" = 500,
#     "CP002168 Escherichia coli UM146 plasmid pUM146, complete sequence. 114.55 KB" = 699
    "GQ227366 Influenza A virus (A/pika/Qinghai/BI/2007(H5N1)) segment 1 polymerase PB2 (PB2) gene, complete cds. 2.31 KB" = 8000
);
# let w2 = list(
#     "AP014857 Escherichia albertii DNA, complete genome, strain: EC06-170. 4.66 MB" = 2000,
#     "CP002967 Escherichia coli W, complete genome. 4.90 MB" = 200
# );
# let w3 = list(
#     "CP002967 Escherichia coli W, complete genome. 4.90 MB" = 1000
# );
# let w4 = list(
#     "CP001671 Escherichia coli ABU 83972, complete genome. 5.13 MB" = 3000,
#     "CP001637 Escherichia coli DH1, complete genome. 4.63 MB" = 1500,
#     "CP002168 Escherichia coli UM146 plasmid pUM146, complete sequence. 114.55 KB" = 699
# );

writeLines(names, con = "Z:/names.txt");

for(i in 1:5) {
    let reads1 = FastQ::simulate_reads(genomes, n= 10000, genome_weights = w1, len = [1600,2100]);
    let reads2 = FastQ::simulate_reads(genomes, n= 10000, genome_weights = NULL);
    let reads3 = FastQ::simulate_reads(genomes, n= 10000, genome_weights = NULL);
    let reads4 = FastQ::simulate_reads(genomes, n= 10000, genome_weights = NULL);
    let save1 = sprintf("Z:/test/w1_%s.fq", i);
    let save2 = sprintf("Z:/test/w2_%s.fq", i);
    let save3 = sprintf("Z:/test/w3_%s.fq", i);
    let save4 = sprintf("Z:/test/w4_%s.fq", i);

    write.fastq(reads1, file = save1);
    write.fastq(reads2, file = save2);
    write.fastq(reads3, file = save3);
    write.fastq(reads4, file = save4);
}

