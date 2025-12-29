require(GCModeller);

imports "bioseq.fasta" from "seqtoolkit";

setwd(@dir);

# get fasta sequence data
let seqs = read.csv("./protein_classification.csv", row.names = 1, check.names = FALSE);
let sgt  = seq_sgt(moltype = "prot");
# get sequence embedding result
let vec  = sgt |> seq_vector(seqs);

# run data analysis on the generated embedding vectors
# embedding the raw matrix from high dimensional space
# into latent space
let latent = prcomp(vec, scal=TRUE,center=TRUE, pc=6);
# run clustering
let clusters = kmeans(latent, centers = 3, bisecting = TRUE);

print(as.data.frame(clusters), max.print = 6);

