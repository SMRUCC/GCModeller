imports "GSEA" from "gseakit.dll";
imports "GCModeller" from "GCModeller_cli.dll";

let enrichment as string = ?"--enrichment";
let go as string = ?"--go";
let KOBAS.convert as string = `${dirname(enrichment)}/${basename(enrichment)}.KOBAS.csv`;

enrichment
:> read.enrichment
:> as.KOBAS_terms
:> 
