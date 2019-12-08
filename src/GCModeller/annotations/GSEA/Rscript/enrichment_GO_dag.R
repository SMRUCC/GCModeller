imports "GSEA" from "gseakit.dll";
imports "gokit.file" from "gokit.dll";

let go <- ?"--go" :> read.go_obo; 
let terms <- ?"--enrichment";
let save.png <- ?"--save" || `${dirname(terms)}/${basename(terms)}.go_dag.png`;

terms 
:> read.enrichment
:> enrichment.draw.go_dag(go = go)
:> save.graphics(file = save.png);


