imports ["geneExpression", "sampleInfo"] from "phenotype_kit";
imports "magnitude" from "phenotype_kit";
imports "bioseq.patterns" from "seqtoolkit";
imports "umap" from "MLkit";
imports "datasetKit" from "MLkit";

setwd(@dir);

let raw = geneExpression::load.expr(file = "./all_counts.csv");

str(dims(raw));

let sgt   = SGT();
let pack  = encode.seqPack(raw, briefSet = FALSE);
# let graph = as.seq_graph(pack);
# let view = as.data.frame(graph);

let view = fit(sgt, pack, df = TRUE);

print(view);

let manifold = umap(view, dimension = 3);
let map = as.data.frame(manifold$umap, labels = manifold$labels, dimension = ["x","y","z"]);

let labels = rownames(map);
let groups = guess.sample_groups(labels, maxDepth = TRUE);

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

require(ggplot);

options(strict = FALSE);

bitmap(file = `./counts_UMAP3d.png`, size = [4800, 3600]) {
    
	let data = map;
	# data[, "class"] = `class_${rownames(data)}`;
	
	# colorset = {
	# class_5:"red",
	# class_0:"blue",
	# class_4:"black",
	# class_1:"yellow",
	# class_9:"green",
	# class_2:"skyblue",
	# class_3:"lime",
	# class_6:"orange",
	# class_7:"steelblue",
	# class_8:"brown"
	# };
	
	print(unique(data[, "class"]));
	
	# create ggplot layers and tweaks via ggplot style options
	ggplot(data, aes(x = "x", y = "y" , z = "z")
    , padding = "padding:250px 500px 100px 100px;")
	+ geom_point(aes(color = "class"), color = "paper", shape = "triangle", size = 80)
	# + view_camera(angle = [31.5,65,125], fov = 100000)
	+ ggtitle("Scatter UMAP 3D")
	# + theme_default()
	;

}