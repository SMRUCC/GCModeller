# demo script for DEM kegg metabolites enrichment analysis 

imports "GCModeller" from "GCModeller_cli.dll";

require(dataframe);

# cli usage for this demo R script
# R# ./enrichment.R --data DEM.csv --background GSEA.background_model.Xml --save result_folder 

let dem                  <- [?"--data"] :> read.dataframe(mode = "numeric");
let background as string <- ?"--background";
let out as string        <- ?"--save";
let cols                 <- dem :> dataset.colnames;
let kit                  <- list(
    eggHTS   = eggHTS()   :> as.object, 
    profiler = profiler() :> as.object
);

console::progressbar.pin.top();

let [partition, fileName] as string;
let data;
let [up, down] as string;
let [GSEA.save, geneSet] as string;

let doEnrichment as function(id, file) {
    geneSet   <- `${out}/${fileName}/${file}.txt`;
    GSEA.save <- `${out}/${fileName}/${file}.GSEA.txt`;
    id :> writeLines(file = geneSet);
    
    # do enrichment of the kegg metabolites
    # /GSEA /background <clusters.XML> /geneSet <geneSet.txt> [/hide.progress /locus_tag /cluster_id <null, debug_used> /format <default=GCModeller> /out <out.csv>]
    kit$profiler$EnrichmentTest(background, geneSet = geneSet, format = "KOBAS", hide_progress = TRUE, out = GSEA.save);
    # /KEGG.enrichment.plot /in <enrichmentTerm.csv> [/gray /colors <default=Set1:c6> /label.right /pvalue <0.05> /tick 1 /size <2000,1600> /out <out.png>]
    kit$eggHTS$KEGG_enrichment(GSEA.save, pvalue = "0.05", tick = "-1", size = "2300,2000" ); 
}

for(i in 1:length(cols) step 3) {
    # get foldchange value
    partition <- cols[i+1];
    fileName  <- cols[i];
    data      <- dem 
      :> dataset.project(cols = partition) 
      :> as.object;
    
	print(`Do dem projection for '${fileName}'.`);
	
    up   <- data 
      :> which(x -> x$GetItemValue(partition) >= 0.1) 
      :> projectAs(x -> x$ID);
    down <- data 
      :> which(x -> x$GetItemValue(partition) <= neg(0.1)) 
      :> projectAs(x -> x$ID);
    
    doEnrichment(up, "up");
    doEnrichment(down, "down");
    doEnrichment(up << down, "all_DEM");

    print(partition);
}