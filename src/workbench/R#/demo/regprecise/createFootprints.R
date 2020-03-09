imports "TRN.builder" from "phenotype_kit.dll";
imports "annotation.workflow" from "seqtoolkit.dll";

["K:\20200226\TRN\genomics\search\regulators.besthit.csv"] 
:> open.stream(type = "SBH", ioRead = TRUE)
:> besthit.filter
:> regulation.footprint(
	"K:\20200226\TRN\genomics\search\TFBS.summary.csv" :> read.footprints,
	"P:\XCC\Regprecise\Db.Xml" :> read.regprecise
)
:> write.regulations(file = "K:\20200226\TRN\genomics\search\regulations.csv")
;
