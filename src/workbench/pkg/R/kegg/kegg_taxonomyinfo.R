const kegg_taxonomyinfo = function(code = NULL) {
    let taxfile = file.path(@datadir, "organism.txt");
    let taxdata = read.table(taxfile, header = FALSE, row.names = 2, check.names = FALSE);

    colnames(taxdata) <- c("tcode","taxname","taxonomy");

    message("get kegg organism information data:");
    print(taxdata, max.print = 6);

    if (is.null(code) || str_empty(code,TRUE,TRUE)) {
        return(taxdata);
    } else if (tolower(code) == "ko" || code == "*") {
        return(taxdata);
    } else {
        return(taxdata[tolower(code),,drop = TRUE]);
    }
}