imports "TRN.builder" from "phenotype_kit";
imports ["annotation.genbank_kit", "annotation.genomics_context", "annotation.workflow"] from "seqtoolkit";

let regions = "K:\20200226\20200516_gbk\X101SC19112292-Z01-J001_result\res.txt"
:> readText
:> as.promoter.models
:> projectAs(as.object)
;

let tuples = [];

for(a in regions) {
	if (a$numOfPromoters > 0) {
		let id = a$locus_tag;
	
		for(site in a$tfBindingSites) {
			tuples = tuples << list(gene = id, regulator = as.object(site)$regulator);
		}
	}
}

tuples = tuples :> groupBy(a -> a$regulator);

let regulator = tuples :> sapply(a -> a$key);
let n = tuples :> sapply(a -> length(a));
let gene_list = tuples :> sapply(a -> paste(a :> sapply(x -> x$gene), ", "));

data.frame(regulator = regulator, n = n, gene_list = gene_list)
:> write.csv(file = "K:\20200226\20200516_gbk\X101SC19112292-Z01-J001_result\res.csv")
;