require(GCModeller);

imports "pangenome" from "comparative_toolkit";
imports "annotation.terms" from "seqtoolkit";
imports "annotation.workflow" from "seqtoolkit";
imports "GenBank" from "seqtoolkit";

let dir = "K:\pangenome\Candidozyma_auris\result";

let diamond = read_m8(file.path(dir, "proteins.txt"));
let terms = assign_terms(diamond);
let orth = multiple_genome_alignment(terms);
let geneset = read_genetable(file.path(dir,"genes.csv"));
let context = build_context(geneset);
let result = pangenome::analysis(context, orth);

write.csv(sv_table(result ), file = file.path(dir, "sv_table.csv" ));
write.csv(pav_table(result), file = file.path(dir,"pav_table.csv"));
writeLines(report_html(result), con = file.path(dir,"result.html"));