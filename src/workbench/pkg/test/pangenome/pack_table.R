require(GCModeller);

imports "GenBank" from "seqtoolkit";

let src = ?"--genbank" || stop("no genbank source was provided!");
let result = ?"--out" || file.path(src, "result");
let gbff = load_genbanks( list.files(src,"*.gbff" ));
let table = c();

for(gb in gbff) {
    table = c(table, as_tabular(gb));
}

write.csv(table, file = file.path(result,"genes.csv"));
