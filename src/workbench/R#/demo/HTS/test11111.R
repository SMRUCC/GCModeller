imports ["geneExpression", "sampleInfo"] from "phenotype_kit";
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

let labels = rownames(map);
let groups = guess.sample_groups(labels);

map[, "class"] = as.character(sapply(labels, function(lb) {
    for(group_id in names(groups)) {
        if (lb in groups[[group_id]]) {
            return(group_id);
            break;
        }
    }

    "NA";
}));

write.csv(map ,file = "./counts_3d.csv", row.names = TRUE);