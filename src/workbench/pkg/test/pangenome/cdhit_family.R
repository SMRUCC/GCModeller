require(GCModeller);

imports "bioseq.fasta" from "seqtoolkit";
imports "kmers" from "seqtoolkit";
imports "pangenome" from "comparative_toolkit";

let dir as string = ?"--dir" || stop("no analysis data provided!");
let proteins = read.fasta(file.path(dir, "proteins.faa"));
let family_result = cdhit_clusters(proteins);
let orth = multiple_genome_alignment( family_groups(family_result$clusters));
let geneset = read_genetable(file.path(dir,"genes.csv"));
let context = build_context(geneset,uniqueByAcc=TRUE);
let result = pangenome::analysis(context, orth);

write.csv(genetic_distance(result), file = file.path(dir, "genetic_distance.csv"));
write.csv(pav_matrix(result), file = file.path(dir, "pav_matrix.csv"));
write.csv(curve_data(result), file = file.path(dir, "pangenome_curve.csv"));
write.csv(sv_table(result ), file = file.path(dir, "sv_table.csv" ));
write.csv(pav_table(result), file = file.path(dir,"pav_table.csv"));
writeLines(report_html(result), con = file.path(dir,"result.html"));
