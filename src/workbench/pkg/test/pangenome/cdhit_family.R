require(GCModeller);

imports "bioseq.fasta" from "seqtoolkit";
imports "kmers" from "seqtoolkit";
imports "pangenome" from "comparative_toolkit";

let dir = "K:\pangenome\Candidozyma_auris\result";
let proteins = read.fasta(file.path(dir, "proteins.faa"));
let family_result = cdhit_clusters(proteins);
let orth = family_groups(family_result$clusters);
let geneset = read_genetable(file.path(dir,"genes.csv"));
let context = build_context(geneset,uniqueByAcc=TRUE);
let result = pangenome::analysis(context, orth);

write.csv(sv_table(result ), file = file.path(dir, "sv_table.csv" ));
write.csv(pav_table(result), file = file.path(dir,"pav_table.csv"));
writeLines(report_html(result), con = file.path(dir,"result.html"));