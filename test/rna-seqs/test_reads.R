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
    "GQ227366 Influenza A virus (A/pika/Qinghai/BI/2007(H5N1)) segment 1 polymerase PB2 (PB2) gene, complete cds. 2.31 KB" = 8000,
    "KF226106 Influenza A virus (A/Jiangsu/2/2013(H7N9)) segment 7 matrix protein 2 (M2) and matrix protein 1 (M1) genes, complete cds. 1020bp" = 3000,
    "KF226109 Influenza A virus (A/Jiangsu/2/2013(H7N9)) segment 8 nuclear export protein (NEP) and nonstructural protein 1 (NS1) genes, complete cds. 906bp" = 2000,
    "GQ227365 Influenza A virus (A/pika/Qinghai/BI/2007(H5N1)) segment 2 polymerase PB1 (PB1) and PB1-F2 protein (PB1-F2) genes, complete cds. 2.34 KB" = 3000,
    "NC_002023 Influenza A virus (A/Puerto Rico/8/1934(H1N1)) segment 1, complete sequence. 2.34 KB"=5000,
"NC_002021 Influenza A virus (A/Puerto Rico/8/1934(H1N1)) segment 2, complete sequence. 2.34 KB"=1000,
"NC_002022 Influenza A virus (A/Puerto Rico/8/1934(H1N1)) segment 3, complete sequence. 2.23 KB"=6000,
"NC_002017 Influenza A virus (A/Puerto Rico/8/1934(H1N1)) segment 4, complete sequence. 1.78 KB"=2000,
"NC_002019 Influenza A virus (A/Puerto Rico/8/1934(H1N1)) segment 5, complete sequence. 1.56 KB"=5000,
"NC_002018 Influenza A virus (A/Puerto Rico/8/1934(H1N1)) segment 6, complete sequence. 1.41 KB"=4000,
"NC_002016 Influenza A virus (A/Puerto Rico/8/1934(H1N1)) segment 7, complete sequence. 1.03 KB"=2500,
"NC_002020 Influenza A virus (A/Puerto Rico/8/1934(H1N1)) segment 8, complete sequence. 890bp"=3000
    # "NC_002204 Influenza B virus RNA 1, complete sequence. 2.37 KB" = 500,
    # "NC_002205 Influenza B virus (B/Lee/1940) segment 2, complete sequence. 2.31 KB" = 3000,
    # "NC_002206 Influenza B virus (B/Lee/1940) segment 3, complete sequence. 2.20 KB" = 7000,
    # "NC_002207 Influenza B virus (B/Lee/1940) segment 4, complete sequence. 1.88 KB" = 4000,
    # "NC_002208 Influenza B virus (B/Lee/1940) segment 5, complete sequence. 1.84 KB" = 300
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

