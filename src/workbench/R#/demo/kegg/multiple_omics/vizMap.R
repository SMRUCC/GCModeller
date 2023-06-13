require(GCModeller);
require(xlsx);

imports "report.utils" from "kegg_kit";

[@info "The file path to the multiple omics data association result 
        table file. This data input file could be a csv/txt table file 
        or xlsx excel table file. For xlsx excel table file the first 
        sheet table will be used for the data rendering."]
[@config "file/associate_mt"]
const associate_mt = ?"--associate_mt" || stop("No data for kegg pathway rendering was provided!");
[@info "The field name for gets the kegg compounds highlights."]
const fd_compounds = ?"--compounds"    || "Compounds_KO";
[@info "The field name for gets the gene highlights."]
const fd_genes     = ?"--genes"        || "Genes_KO";
[@info "The field name for gets the protein highlights."]
const fd_proteins  = ?"--proteins"     || "Proteins_KO";

const inputs = {
    if (file.ext(associate_mt) == "xlsx") {
        read.xlsx(
            file = associate_mt, 
            sheetIndex = 1, 
            row.names = 1, 
            check.names = FALSE
        );
    } else {
        read.csv(
            file = associate_mt, 
            row.names = 1, 
            check.names = FALSE, 
            tsv = file.ext(associate_mt) == "txt"
        );
    }
}

print("Peeks of the input kegg pathway map highlights dataset:");
print(inputs);

const get_highlights = function(fd_name) {
    if (fd_name in inputs) {
        strsplit(inputs[, fd_name], "\s*,\s*")
        |> unlist()
        |> lapply(function(hl) {
            let parse = strsplit(hl, "\s*[:]\s*");
            let highlight = list(id = parse[1], color = parse[2]);

            highlight;
        })
        |> lapply(hl -> hl$color, names = hl -> hl$id)
        ;
    } else {
        return(NULL);
    }
};
const hl_compounds = get_highlights(fd_compounds);
const hl_genes = get_highlights(fd_genes);
const hl_proteins = get_highlights(fd_proteins);

str(hl_compounds);
str(hl_genes);
str(hl_proteins);
