# script for run GCModeller vCell Compiler to create
# virtual cell model file

imports "vcellkit.compiler" from "vcellkit.dll";
imports "TRN.builder" from "phenotype_kit.dll";
imports ["annotation.genomics", "annotation.workflow", "annotation.genbank_kit"] from "seqtoolkit.dll";

let kegg.repo <- kegg(
	compounds  = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\KEGG_cpd", 
	maps       = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\KEGG_maps", 
	reactions  = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\br08201", 
	glycan2Cpd = "E:\smartnucl_integrative\biodeepdb_v3\KEGG\KEGG_cpd.glycan.compoundIds.json" :> read.list(ofVector = TRUE) 
);
let maps = "K:\20200226\X101SC19112292-Z01-J001-B1-16_TR_result\X101SC19112292-Z01-J001-B1-16_results\0.SuppFiles\KO.csv"
:> open.stream(type = "BBH", ioRead = TRUE)
:> geneKO.maps
;

let regulations = read.regulations("K:\20200226\TRN\genomics\search\regulations.csv");
let genome <- list(genome = read.genbank("K:\20200226\IGV_data\assembly.gb"));
let logfile as string = "K:\20200226\metabolism\vcell\model.log";
let model = genome
:> assembling.genome(maps)
:> assembling.metabolic(maps, kegg.repo)
:> assembling.TRN(regulations = regulations)
:> vcell.markup(genome, kegg.repo, regulations, logfile = logfile)
;

model
:> xml
:> writeLines(con = "K:\20200226\metabolism\vcell\model.Xml")
;

zip(model, "K:\20200226\metabolism\vcell\vcellModel.zip"); 
