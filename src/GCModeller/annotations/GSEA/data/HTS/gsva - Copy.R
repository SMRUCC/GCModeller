require(GCModeller);
require(ggplot);

imports ["background", "GSVA"] from "gseakit";
imports "visualPlot" from "visualkit";
imports "geneExpression" from "phenotype_kit";

expr0    = load.expr("E:\plot\metabolome_pos.csv");
metainfo = read.csv("E:\plot\doMSMSalignment.report1.csv", row.names = NULL);
# metaSet  = read.csv(`${@dir}/kegg_enrichment.xls`, row.names = NULL, check.names = FALSE, tsv = TRUE);
# metaSet[, "names"] = NULL;

metaSet = GCModeller::kegg_maps(rawMaps = FALSE)  
|> background::metabolism.background() 
|> as.geneSet()
;

print("view background:");
str(metaSet);

metainfo[, "KEGG"] = unique.names(metainfo[, "KEGG"]);
metainfo = as.list(metainfo, byrow = TRUE);
names(metainfo) = sapply(metainfo, r -> r$KEGG);
metainfo = lapply(metainfo, r -> r$ID);

str(metainfo);

getCluster = function(kegg_id) {
	# kegg_id = strsplit(term$compounds, ";");
    names = sapply(kegg_id, id -> metainfo[[id]]);
    names;
}

normalization = function(x) {
	(x - min(x)) / (max(x) - min(x));
}

run_gsva = function() {
	print("run gsva analysis...");
    
    metaSet = metaSet |> lapply(r -> getCluster(kegg_id = r));

    # print("view of the background dataset:")
    # str(metaSet)

    scores = GSVA::gsva(z_score(expr0), metaSet);

    # print("preview of your gsva score matrix:")
    # print(as.data.frame(scores), max.print = 13)

    write.expr_matrix(scores, file = "E:\plot/gsva.csv", id = "pathway Name");

    scores = as.data.frame(scores);
    
    for(i in colnames(scores)) {
	    print(`normalize score: ${i}`);
        scores[, i] = normalization(scores[, i]);
	}
       
    scores;
}

gsva_score = @profile {
	run_gsva();
};

diff = function(name, g1, g2) {
	mean1  = mean(g1);
    mean2  = mean(g2);
    sd1    = sd(g1);
    sd2    = sd(g2);
    t      = as.object(t.test(g1, g2));
    pvalue = t$Pvalue;
    t      = t$TestValue;
    
    list(name, mean1, mean2, sd1, sd2, t, pvalue);
}

run_diff = function(gsva_score) {
    print("run group diff between GQ and I...");

    # test group diff
    A01 = ["G-Q-1","G-Q-2","G-Q-3"];
    CK  = ["I-1","I-2","I-3"];

    A01       = gsva_score[, A01];
    CK        = gsva_score[, CK];
    pathNames = rownames(gsva_score);
        
    compares   = lapply(1:nrow(gsva_score), i -> diff(pathNames[i], unlist(as.list(A01[i, ])), unlist(as.list(CK[i, ]))));
    foldchange = sapply(compares, i -> (i$mean1) / (i$mean2));
    log2fc     = log(foldchange, 2);
    m1         = sapply(compares, i -> i$mean1);
    m2         = sapply(compares, i -> i$mean2);
    sd1        = sapply(compares, i -> i$sd1);
    sd2        = sapply(compares, i -> i$sd2);
    t          = sapply(compares, i -> i$t);
    pvalue     = sapply(compares, i -> i$pvalue);
    pathName   = sapply(compares, i -> i$name);

    data.frame(
		pathNames = pathName, 
		mean_A01 = m1, 
		mean_CK = m2, 
		sd_A01 = sd1, 
		sd_CK = sd2, 
		foldchange = foldchange, 
		log2fc = log2fc, 
		t = t, 
		pvalue = pvalue
	);
}	

print(" ~done!");
gsva_diff = run_diff(gsva_score);
print(gsva_diff, max.print = 13);
write.csv(gsva_diff, file = `E:\plot/GQ_vs_I_gsva.csv`, row.names = FALSE);

# do data visualization


data = read.csv(`E:\plot/GQ_vs_I_gsva.csv`, row.names = NULL);
data = data[data[, "t"] > 0 || data[, "t"] < -6, ];

print("previews of the gsva diff result between A01 and CK group:");
print(data, max.print = 6);

data = GSVA::matrix_to_diff(data, "pathNames", "t","pvalue");

bitmap(file = `E:\plot/GQ_vs_I.png`) {
   plot(data, padding = "padding: 100px 200px 200px 200px", size = [3300, 3600]);
}


print(" ~~done!");
profile = as.data.frame(profiler.fetch());

print("total time:");
print(timespan(sum(profile[, "ticks"])));

cat("\n");

print(profile, max.print = 13);

write.csv(profile, file = `E:\plot/profile.csv`, row.name = TRUE);

profile[, "index"] = 1:nrow(profile);

# data visualize
bitmap(file = `E:\plot/memory_delta.png`) {
    ggplot(profile, padding = "padding: 200px 800px 250px 250px;", width = 4000, height = 2400) 
    + geom_line( aes(x = "index", y = "memory_delta"), width = 8)
	+ xlab("time(ticks)")
	+ ylab("memory_delta(MB)")
	+ ggtitle("Memory Delta Size")
    ;
}