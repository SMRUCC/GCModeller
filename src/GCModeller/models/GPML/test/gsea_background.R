# Slenter DN, Kutmon M, Hanspers K, Riutta A, Windsor J, Nunes N, Mélius J, Cirillo E, Coort SL, Digles D, Ehrhart F, Giesbertz P, Kalafati M, Martens M, Miller R, Nishida K, Rieswijk L, Waagmeester A, Eijssen LMT, Evelo CT, Pico AR, Willighagen EL.
# WikiPathways: a multifaceted pathway database bridging metabolomics to other omics research Nucleic Acids Research, (2017)
# DOI: 10.1093/nar/gkx1064  PMCID: PMC5753270

imports "PathVisio" from "cytoscape_toolkit";
imports "background" from "gseakit";

"D:\GCModeller\src\GCModeller\models\GPML\data"
:> list.files(pattern = "*.gpml")
:> lapply(function(filepath) {
	
	let pathway = as.object(read.gpml(filepath));
	let lipids = pathway :> nodes.table;
	
	lipids = lipids[lipids[, "database"] == "LIPID MAPS", ];
	lipids = lipids[lipids[, "dbref"] != "", ];

	lipids :> gsea.cluster(
		clusterId   = basename(filepath),
		clusterName = pathway$Name,
		desc        = as.object(pathway$Comment)$Text,
		id          = "dbref"
	);
})
:> as.background(
	background_size = 20000,
	name            = "LipidMaps",
    tax_id          = "lipid",
    desc            = "lipidmaps enrichment background" 
)
:> xml
:> writeLines(con = `${!script$dir}/lipidmaps.Xml`)
;