
.onLoad <- function(libname, pkgname) {
    cat("\n\n");
    cat("  GCModeller: genomics CAD(Computer Assistant Design) Modeller System\n");
    cat("                                author by: xie.guigang@gcmodeller.org\n");
    cat("\n");
    cat("                           (c) 2021 | SMRUCC genomics - GuiLin, China\n");
    cat("\n\n");

    require(XML);
    require(magrittr);
}

.flashLoad <- function() .onLoad(NULL, NULL);