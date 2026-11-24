imports ["annotation.genbank_kit", "annotation.genomics", "annotation.terms"] from "seqtoolkit";

setwd(!script$dir);

let genes = "K:\Xanthomonas_campestris_8004_uid15\genbank\CP000050.1.txt"
:> read.genbank
:> genome.genes
:> sapply(gene -> as.object(gene))
;

let product as string = sapply(genes, gene -> gene$Product);
let debug = data.frame(
	locus_tag = sapply(genes, gene -> gene$Synonym),
	geneName  = geneNames(product),
	product   = product
);

debug :> write.csv(file = "./geneNames.csv", row_names = FALSE);