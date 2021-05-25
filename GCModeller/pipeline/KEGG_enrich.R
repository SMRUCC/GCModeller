imports "uniprot" from "seqtoolkit";
imports ["background", "GSEA", "profiles"] from "gseakit";
imports "visualPlot" from "visualkit";
imports "brite" from "kegg_kit";

[@info "a id list in plain text format, each line in the text file is the gene id"]
const testId_input as string = ?"--id"  || stop("a id list must be provided!");
const outputdir as string    = ?"--out" || dirname(testId_input);

# map gene id to uniprot id
const toUniprot = [`${dirname(@script)}/uniprot-taxonomy__Escherichia+coli+(strain+K12)+[83333]_-filtered---.xml`]
|> open.uniprot 
|> id_unify(target = "gene:b\d+", id = readLines(testId_input))
;

const kegg_background = [`${dirname(@script)}/eco00001.keg`]
|> brite::parse
|> KO.background(
	maps   = brite::parse("ko00001"), 
	id_map = "b\d+"
)
;

print(kegg_background);

write.enrichment(file = `${outputdir}/enrich.csv`, format = "KOBAS") {
	const enrich = kegg_background
	|> enrichment(toUniprot, showProgress = FALSE)
	|> enrichment.FDR
	|> as.KOBAS_terms
	;
	
	bitmap(file = `${outputdir}/KEGG.png`) {
		enrich 
		|> which(i -> as.object(i)$CorrectedPvalue <= 0.05)
		|> KEGG.enrichment.profile
		|> category_profiles.plot(
			title      = "KEGG Enrichment",
            axis_title = "-log10(p-value)",
			size       = [3300,2700]
		)
		;
	}
	
	enrich;
}