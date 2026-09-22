imports "pangenome" from "comparative_toolkit";

let dir as string = ?"--dir" || stop("no analysis data provided!");
let geneset = read_genetable(file.path(dir, "genes.csv"));

print(length(geneset));

let context = build_context(geneset, uniqueByAcc = TRUE);

print("context built ok");
