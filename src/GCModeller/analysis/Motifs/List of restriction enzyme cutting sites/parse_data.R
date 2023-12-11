imports "Html" from "webKit";

setwd(@dir);

for(file in list.files("./txt")) {
    let html = readText(file);
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

        for(name in colnames(par)) {
            vs = par[, name];
            vs = vs |> gsub("\[\d+\]", "", regexp = TRUE);
            par[, name] = vs;
        }

        i = par$recognition != "";
        par = par[i, ];

        offset = nrow(exports) + 1;
        rownames(par) = offset:(offset + nrow(par) - 1);
        exports = rbind(exports, par);
    }

    print(exports);

    let filename = basename(file);
    filename = gsub(filename, "[#].+", "", regexp = TRUE);
    filename = trim(filename, "_ ");
    filename = gsub(filename, "%E2%80%93", "-");

    write.csv(exports, file = `./sites/${filename}.csv`, row.names = FALSE);
}