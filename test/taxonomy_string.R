require(GCModeller);

imports "taxonomy_kit" from "metagenomics_kit";

let tree = Ncbi.taxonomy_tree("G:\metagenomics-llms\data\taxdump");

for(let taxid in c(629)) {
    print(tree |> lineage(tax = taxid) |> biom.string);
}