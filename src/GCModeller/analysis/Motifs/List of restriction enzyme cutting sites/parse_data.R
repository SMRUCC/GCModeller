imports "Html" from "webKit";

setwd(@dir);

for(file in list.files("./txt")) {
    let html = readText(file) |> gsub("\[\d+\]", "", regexp = TRUE);
    let data = Html::tables(html)
    |> which(d -> ncol(d) > 4)
    ;
    let exports = NULL;

    for(par in data) {
        par = data.frame(
            enzyme = par$Enzyme, 
            pdb = par$"PDB code", 
            organism = par$"Source", 
            recognition = par$"Recognition sequence", 
            cut = par$Cut, 
            isoschizomers = par$Isoschizomers
        );
        offset = nrow(exports) + 1;
        rownames(par) = offset:(offset + nrow(par) - 1);
        exports = rbind(exports, par);
    }

    print(exports);

    write.csv(exports, file = `./sites/${basename(file)}.csv`, row.names = FALSE);
}