imports ["annotation.genbank_kit", "annotation.genomics"] from "seqtoolkit";

const test.gbff as string = "K:\bacterials\GCA_001931535.1_ASM193153v1_genomic.gbff.gz";

using gbff as open.gzip(test.gbff) {
	
	gbff 
	:> populate.genbank 
	:> projectAs(genome.genes) 
	:> unlist 
	:> as.vector(mode = "gene_info") 
	:> write.PTT_tabular(file = "X:\test.txt")
	;

}