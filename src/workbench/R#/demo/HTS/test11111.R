imports "geneExpression" from "phenotype_kit";
imports "magnitude" from "phenotype_kit";
imports "bioseq.patterns" from "seqtoolkit";
imports "umap" from "MLkit";


setwd(@dir);

let raw = geneExpression::load.expr(file = "./all_counts.csv");

str(dims(raw));

let pack = encode.seqPack(raw);
let graph = as.seq_graph(pack);

let view = as.data.frame(graph);

print(view);

let manifold = umap(view, dimension = 3);
let map = as.data.frame(manifold$umap, labels = manifold$labels, dimension = ["x","y","z"]);

write.csv(map ,file = "./counts_3d.csv", row.names = TRUE);