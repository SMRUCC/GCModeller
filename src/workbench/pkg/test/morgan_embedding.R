require(umap);

imports "clustering" from "MLkit";

setwd(@dir);

let mat = read.csv("./morgan.csv", row.names = 1);
let latent = umap(mat, dimension=9,numberOfNeighbors =32, method = "TanimotoFingerprint"); 
# run clustering
let clusters = kmeans(as.data.frame( latent$umap,labels = latent$labels), 
    centers = 6, 
    bisecting = TRUE);

clusters = as.data.frame(clusters);

message("the input sequence data is embedding as result:");
print(clusters, max.print = 13);

bitmap(file = "protein_morgan.png", size = c( 1200,  800)) {
    plot(clusters$dimension_1, clusters$dimension_2, 
        class = clusters$Cluster, 
        colors = "paper", 
        point.size = 8,
        grid.fill = "white",
        padding = "padding: 5% 10% 15% 15%;"
    );
}
