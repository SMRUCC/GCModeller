require(GCModeller);
require(umap);

imports "bioseq.fasta" from "seqtoolkit";
imports "clustering" from "MLkit";

setwd(@dir);

# get fasta sequence data
let seqs = read.csv("./protein_classification.csv", row.names = 1, check.names = FALSE);
let sgt  = seq_sgt(moltype = "prot", kappa = 1,lengthsensitive= FALSE );
# get sequence embedding result
let vec  = seq_vector(sgt, as.fasta(seqs), as.dataframe = TRUE);

# run data analysis on the generated embedding vectors
# embedding the raw matrix from high dimensional space
# into latent space
let latent = umap(vec, dimension=9,numberOfNeighbors =3, method = "SpectralCosine"); 
# run clustering
let clusters = kmeans(as.data.frame( latent$umap,labels = latent$labels), 
    centers = 3, 
    bisecting = TRUE);

clusters = as.data.frame(clusters);

message("view of the raw sequence data input");
print(seqs, max.print = 6);
message("the input sequence data is embedding as result:");
str(latent);
print(clusters, max.print = 13);

bitmap(file = "protein_classification.png", size = c( 1200,  800)) {
    plot(clusters$dimension_1, clusters$dimension_2, 
        class = clusters$Cluster, 
        colors = "paper", 
        point.size = 6,
        grid.fill = "white",
        padding = "padding: 5% 10% 15% 20%;"
    );
}

