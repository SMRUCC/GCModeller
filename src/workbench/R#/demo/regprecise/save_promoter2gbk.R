imports "TRN.builder" from "phenotype_kit";
imports ["annotation.genbank_kit", "annotation.genomics_context", "annotation.workflow"] from "seqtoolkit";

let regions = "K:\20200226\20200516_gbk\X101SC19112292-Z01-J001_result\res.txt"
:> readText
:> as.promoter.models
:> lapply(a => a, names = a => as.object(a)$locus_tag)
;

print(names(regions));

let mapping = "K:\20200226\TRN\1025_mapping.csv"
:> open.stream(type = "Mapping", ioRead = TRUE)
:> projectAs(as.object)
:> groupBy(a -> a$Reference)
:> lapply(function(a) {
	if (length(a) == 1) {
		a[1]$Reads.Query;
	} else {
		let ref = sapply(a, x -> x$Reads.Query);
		let scores = sapply(a, x -> x$"Score(bits)");
		
		ref[which.max(scores)];
	}
	
}, names = g -> g$key);

str(mapping);

let templateGenbank = "K:\20200226\20200516_gbk\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal.gbk"
:> read.genbank
;

let genes = templateGenbank :> enumerateFeatures :> projectAs(as.object) :> which(a -> a$KeyName == "CDS");
let promoter;

for(a in genes) {
	let location = as.object(a$Location)$ContiguousRegion;
	let db_xref = a$Query("db_xref");
	let id = mapping[[db_xref]];
	
	if (is.null(id)) {
		next;
	} else {
		promoter = regions[[id]];
	}
		
	if (is.null(promoter) || (as.object(promoter)$numOfPromoters == 0)) {
		next;
	} else {
		let direct = is.forward(location) ? -1 : 1;
	
		for(site in as.object(promoter)$tfBindingSites) {
			let tfbs_loci = offset(location, direct * as.object(promoter)$promoterPos);
			
			# create feature
			site = as.object(site);
			tfbs_loci = feature("Promoter", tfbs_loci, list(
				family = site$regulator,
				oligonucleotides = site$oligonucleotides,
				score = site$score,
				target = id,
				target_dbxref = db_xref
			));
			# and then insert into genbank file
			templateGenbank :> add.feature(tfbs_loci);
		}	
	}
	
	print(id);
}

templateGenbank :> write.genbank(file = "K:\20200226\20200516_gbk\X101SC19112292-Z01-J001_result\Yersinia pseudotuberculosis (Pfeiffer) Smith and Thal_add_promoter_sites.gbk");
