require(GCModeller);

imports "pangenome" from "comparative_toolkit";
imports "annotation.terms" from "seqtoolkit";
imports "GenBank" from "seqtoolkit";

let files = list.files("N:\GMNDesigner\SuperCC\models_new\release", pattern = "*.csv", recursive =TRUE);
let genomes = files[basename(files) == "gene_table"];

genomes <- as.list(genomes, names = basename(dirname(genomes)));
files <- files[basename(files) == "enzymes"];
files <- as.list(files, names = basename( dirname(files)))
|> lapply(path -> read.csv(path, row.names = NULL, check.names = FALSE, stringsAsFactors = FALSE))
|> lapply(df -> rank_term(df$gene_id, df$EC, df$Score, df$proteinName))
;

let pangenome = build_context(lapply(genomes, path -> read_genetable(path)), soft_core_threshold = 0.95); 
let result = pangenome |> analysis(orthologSet = files);

write.csv(sv_table(result ), file = "N:\GMNDesigner\SuperCC\sv_table.csv" );
write.csv(pav_table(result), file = "N:\GMNDesigner\SuperCC\pav_table.csv");
writeLines(report_html(result), con = "N:\GMNDesigner\SuperCC\result.html");