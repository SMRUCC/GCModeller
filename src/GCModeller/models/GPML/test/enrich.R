imports ["background", "GSEA"] from "gseakit";

const lipidmaps = read.background(`${!script$dir}/lipidmaps.Xml`);

print(lipidmaps);

lipidmaps
:> enrichment(geneSet = readLines("F:\lipids\tables\20201105-HCD GEN\lipidmaps.txt"))
:> enrichment.FDR
:> write.enrichment(file = "F:\lipids\tables\20201105-HCD GEN\lipidmaps_enrich.csv", format = "GCModeller")
;