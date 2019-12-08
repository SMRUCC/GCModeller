imports "GSEA" from "gseakit.dll";
imports "GCModeller" from "GCModeller_cli.dll";

let enrichment as string = ?"--enrichment";
let go as string = ?"--go";
let KOBAS.convert as string = `${dirname(enrichment)}/${basename(enrichment)}.KOBAS.csv`;
let eggHTS = eggHTS() :> as.object;

enrichment
:> read.enrichment
:> as.KOBAS_terms
:> write.enrichment(file = KOBAS.convert);

# /Go.enrichment.plot /in <enrichmentTerm.csv> [/bubble /r "log(x,1.5)" /Corrected /displays <default=10> /PlantRegMap /label.right /label.color.disable /label.maxlen <char_count, default=64> /colors <default=Set1:c6> /gray /pvalue <0.05> /size <2000,1600> /tick 1 /go <go.obo> /out <out.png>]
eggHTS$GO_enrichmentPlot(
    in = KOBAS.convert,
    go = go
);

