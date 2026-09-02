require(GCModeller);

imports "pangenome" from "comparative_toolkit";
imports "annotation.terms" from "seqtoolkit";

let files = list.files("N:\GMNDesigner\SuperCC\models_new\release", pattern = "*.csv", recursive =TRUE);
files = files[basename(files) == "enzymes"];
files = as.list(files, names = basename( dirname(files)));
files = lapply(files, path -> read.csv(path, row.names = NULL, check.names = FALSE, stringsAsFactors = FALSE));
files = lapply(files, df -> rank_term(
    df$gene_id, `${df$EC} [${df$proteinName}]`, df$Score, df$SourceIDs
));

str(files);