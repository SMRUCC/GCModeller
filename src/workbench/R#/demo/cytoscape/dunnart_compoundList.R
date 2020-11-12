imports ["dunnart", "cytoscape.kegg"] from "cytoscape_toolkit";
imports "kegg.repository" from "kegg_kit";

require(igraph);

setwd(!script$dir);

let compounds = read.csv("doMSMSalignment.report1.csv")[, "KEGG"] :> as.character :> unique;
let maps = read.KEGG_pathway("D:\biodeep\biodeep_pipeline\Biodeep_Rpackage\gsea\kegg\hsa");

print(compounds);

"D:\biodeep\biodeepdb_v3\KEGG\br08201.csv"
:> reactions.table
:> compounds.network(
	compounds             = compounds[!(compounds in ["NULL", "NA"])], 
	strictReactionNetwork = FALSE,
	enzymeBridged         = FALSE,
	extended              = FALSE
)
:> optmize(optmize_iterations = 50)
:> connected_graph
:> network_map(maps, desc = TRUE)
:> json
:> writeLines(con = "./demo2.json")
;
