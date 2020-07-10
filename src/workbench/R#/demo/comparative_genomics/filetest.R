imports ["annotation.genbank_kit", "annotation.genomics"] from "seqtoolkit";

const test.gbff as string = "K:\bacterials\complete.1.genomic.gbff.gz";

using gbff as open.gzip(test.gbff) {
	
	gbff :> populate.genbank :> as.tabular :> 

}