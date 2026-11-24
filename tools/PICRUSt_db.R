require(GCModeller);

imports "microbiome" from "metagenomics_kit";

let otu_tax = parse.otu_taxonomy("J:\gg_13_8_99.gg.tax");
let copy_number_16s = read.csv("J:\16S_13_5_precalculated.tab", row.names = 1, check.names = FALSE, comment.char = NULL, tsv = TRUE);
let savedata = file("J:\ko_13_5_precalculated.PICRUSt", open = "truncate");

print(copy_number_16s, max.print = 6);

copy_number_16s = as.list( copy_number_16s, byrow = TRUE) |> lapply(a -> a$16S_rRNA_Count);

str(copy_number_16s);

otu_tax |> build.PICRUSt_db(
    copyNumbers_16s = copy_number_16s,
    ko_13_5_precalculated = file("J:\ko_13_5_precalculated.tab", open = "open"),
    save = savedata
);

close(savedata);