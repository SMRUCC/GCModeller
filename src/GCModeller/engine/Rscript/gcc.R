imports "vcellkit.compiler" from "vcellkit.dll";
imports "TRN.builder" from "phenotype_kit.dll";
imports ["annotation.genomics", "annotation.workflow"] from "seqtoolkit.dll";

let kegg.repo <- kegg(
	compounds  = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\KEGG_cpd", 
	maps       = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\KEGG_maps", 
	reactions  = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08201", 
	glycan2Cpd = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\KEGG_cpd.glycan.compoundIds.json" :> read.list(ofVector = TRUE) 
);
let maps = "K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\KO.csv"
:> open.stream(type = "BBH", isRead = TRUE)
:> geneKO.maps
;

let regulations = read.regulations("K:\20200226\TRN\genomics\search\regulations.csv");

let genome <- list(genome = ["K:\20200226\IGV_data\genome.gtf"]
	:> read.gtf
	:> as.genbank
);

genome
:> assembling.genome(maps)
:> assembling.metabolic(maps, kegg.repo)
:> assembling.TRN(regulations = regulations)
:> vcell.markup(genome, kegg.repo, regulations)
:> xml
:> writeLines(con = "K:\20200226\metabolism\vcell\model.Xml")
;
