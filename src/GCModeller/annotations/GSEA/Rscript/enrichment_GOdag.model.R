imports "GSEA" from "gseakit.dll";
imports "gokit.file" from "gokit.dll";
imports "igraph" from "R.graph.dll";

let go <- ?"--go" :> read.go_obo; 
let terms <- ?"--enrichment";
let save.dir <- ?"--save" || `${dirname(terms)}/${basename(terms)}.go_dag.network/`;

terms 
:> read.enrichment
:> enrichment.go_dag(go = go)
:> save.network(file = save.dir);