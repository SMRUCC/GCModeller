require(GCModeller);

imports "bioseq.fasta" from "seqtoolkit";
imports "kmers" from "seqtoolkit";

setwd(@dir);

# get fasta sequence data
let seqs = read.csv("./protein_classification.csv", row.names = 1, check.names = FALSE);
let table = tfidf_vectorizer(as.fasta(seqs), k = 6, type = "protein");

write.csv(as.data.frame(table), file = "./tf-idf.csv", row.names = FALSE);


