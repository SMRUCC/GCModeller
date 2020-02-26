imports "network.TRN" from "cytoscape_toolkit.dll";

require(dataframe);

let raw.data as string = ?"--fpkm" || stop("No expression data provided!");
let file.save as string = `${dirname(raw.data)}/${basename(raw.data)}_connections.csv`;

let is_novel_srna_gene = geneId -> (geneId like $"Novel\d+" || geneId like $"sRNA\d+");

raw.data
:> read.dataframe(mode = "numeric")
:> which(a -> !is_novel_srna_gene(as.object(a)$ID))
:> fpkm.connections(cutoff = 0.7)
:> write.csv(file = file.save)
;

