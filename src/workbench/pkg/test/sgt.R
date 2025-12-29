require(GCModeller);

imports "bioseq.fasta" from "seqtoolkit";

setwd(@dir);

# get fasta sequence data
let seqs = read.csv("./protein_classification.csv", row.names = 1, check.names = FALSE);
let sgt  = seq_sgt(moltype = "prot", kappa = 1,lengthsensitive= FALSE );
# get sequence embedding result
let vec  = seq_vector(sgt, as.fasta(seqs), as.dataframe = TRUE);

# run data analysis on the generated embedding vectors
# embedding the raw matrix from high dimensional space
# into latent space
let latent = prcomp(vec, scal=TRUE,center=TRUE, pc=6);
# run clustering
let clusters = kmeans(latent, centers = 3, bisecting = TRUE);

message("view of the raw sequence data input");
print(seqs, max.print = 13);
message("the input sequence data is embedding as result:");
print(as.data.frame(clusters), max.print = 6);



