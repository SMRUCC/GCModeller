require(GCModeller);

imports "bioseq.fasta" from "seqtoolkit";
imports "proteinKit" from "seqtoolkit";

setwd(@dir);

# get fasta sequence data
let seqs = read.csv("./protein_classification.csv", row.names = 1, check.names = FALSE);
let g = kmer_graph(as.fasta(seqs), k = 6);
let vec = lapply(tqdm(g), seq -> kmer_fingerprint(seq,radius = 3, len = 4096));
let table = data.frame(row.names = names(vec));

for(i in 1:4096) {
    table[,`v${i}`] = vec@{i};
}

str(table);

write.csv(table, file = "./morgan.csv");


