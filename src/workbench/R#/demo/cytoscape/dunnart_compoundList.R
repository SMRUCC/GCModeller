imports ["dunnart", "cytoscape.kegg"] from "cytoscape_toolkit";
imports "kegg.repository" from "kegg_kit";

setwd(!script$dir);

let compounds = read.csv("doMSMSalignment.report1.csv")[, "KEGG"] :> as.character :> unique;
let maps = read.KEGG_pathway("D:\biodeep\biodeep_pipeline\Biodeep_Rpackage\gsea\kegg\hsa");

"D:\biodeep\biodeepdb_v3\KEGG\br08201.csv"
:> reactions.table
:> compounds.network(compounds = compounds[!(compounds in ["NULL", "NA"])])
:> network_map(maps)
:> json
:> writeLines(con = "./demo.json")
;
