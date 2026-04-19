const kegg_taxonomyinfo = function(code = NULL) {
    let taxfile = file.path(@datadir, "organism.txt");
    let taxdata = read.table(taxfile, header = FALSE, row.names = 1, check.names = FALSE);

    message("get kegg organism information data:");
    print(taxdata, max.print = 6);

    
}