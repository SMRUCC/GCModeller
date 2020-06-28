imports "kegg" from "bioconductor";
imports "kegg.repository" from "kegg_kit";

reactions.table("D:\biodeep\biodeepdb_v3\KEGG\br08201.csv")
:> write.keggReaction.rds(saveRDS = "D:\biodeep\biodeep_pipeline\KEGGRepository\data\kegg_reactions.rds")