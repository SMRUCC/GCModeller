imports ["bioseq.patterns", "bioseq.fasta"] from "seqtoolkit";

const regprecise = ["K:\20200226\TRN\PWM"]
:> list.files(pattern = "*.json")
:> lapply(read.motifs, names = basename)
;

print(names(regprecise));

let toFasta as function(geneId, seq) {
	sapply(1:length(geneId), function(i) {
		fasta(seq[i], geneId[i])
	});
}

let scan_output = NULL;

for(file in ["K:\20210127\result\两个温度表达差异不显著的基因_包括差异表达基因对应的启动子分析_add_T25vsT37_deg_no.xls",
"K:\20210127\result\25度差异表达的基因_FoldLog值越大25度表达越高_包括差异表达基因对应的启动子分析_add_T25vsT37_deg_up.xls",
"K:\20210127\result\37度差异表达的基因_FoldLog值越小37度表达越高_包括差异表达基因对应的启动子分析_add_T25vsT37_deg_down.xls"]) {
	
	const genelist = read.csv(file, tsv = TRUE, check_names = FALSE, row_names = 1);
	
	# print(head(genelist));
	
	const geneId = rownames(genelist);
	const box15 = toFasta(geneId, genelist[, "Promoter_down_15bp"]);
	const box100 = toFasta(geneId, genelist[, "up 100 bp seq"]);
	
	print(file);
	
	for(scans in lapply(names(regprecise), function(family) {
		cat(family);
	
		let scans = parSapply(regprecise[[family]], function(motif) {			
			append(
				motif :> motif.find_sites(target = box15), 
				motif :> motif.find_sites(target = box100)
			);
		})
		:> unlist
		:> as.data.frame
		;
		
		cat(`...done! ${nrow(scans)} sites\n`);
		
		if (is.null(scans)) {
			NULL;
		} else {
			scans[, "family"] = family;
			scans;	
		}
		
	})) {
	
		scan_output = rbind(scan_output, scans);
	};
}

write.csv(scan_output, file = "K:\20210127\result\TRN2\Regprecise_scans.csv");
