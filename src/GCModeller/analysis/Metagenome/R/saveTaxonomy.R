imports "taxonomy_kit" from "metagenomics_kit";

"F:/"
:> Ncbi.taxonomy_tree
:> as.data.frame
:> write.csv(file = "F:/ncbi_taxonomy.csv", row_name = FALSE)
;