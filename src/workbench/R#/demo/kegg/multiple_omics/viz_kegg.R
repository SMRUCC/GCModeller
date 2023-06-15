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
[@info "the file path to the kegg pathway map bunddle file."]
const map_repo     = ?"--kegg_maps"    || "/opt/biodeep/kegg/KEGG_maps.pack";
[@info "A directory folder path for export the kegg pathway maps highlight result files."]
const outputdir    = ?"--outputdir"    || `${dirname(associate_mt)}/${basename(associate_mt)}_KEGGMaps/`;
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

#' Parse a single color highlight token
#'
const parse_color = function(hl) {
    let parse = strsplit(hl, "\s*[:]\s*");
    let highlight = list(id = parse[1], color = parse[2]);

    highlight;
}
const pathwayId as string = `map${$"\d+"(rownames(inputs))
    |> unlist()
    |> as.integer()
    |> str_pad(5, "left", "0")}`
;
const get_highlights = function(fd_name) {
    if (fd_name in inputs) {
        inputs[, fd_name] 
        |> strsplit("\s*[,;]\s*")        
        |> lapply(function(hl) {
            hl 
            |> lapply(x -> parse_color(hl = x))
            |> lapply(hl -> hl$color, names = hl -> hl$id)
            ;
        }, names = pathwayId)
        ;
    } else {
        return(NULL);
    }
};
const hl_compounds = get_highlights(fd_compounds);
const hl_genes     = get_highlights(fd_genes);
const hl_proteins  = get_highlights(fd_proteins);

print("Do kegg pathway map highlights for:");
print(pathwayId);
print("inspect of the object highlights data:");
str(list(
    compounds = hl_compounds, 
    genes     = hl_genes, 
    proteins  = hl_proteins
));

const KEGG_maps = GCModeller::kegg_maps(rawMaps = FALSE, repo = {
    if (file.exists(map_repo)) {
        map_repo;
    } else {
        system.file("data/kegg/KEGG_maps.zip", package = "GCModeller");
    }
});

for(i in 1:length(pathwayId)) {
    const map_id    = pathwayId[i];
    const compounds = hl_compounds[[map_id]];
    const genes     = hl_genes[[map_id]];
    const proteins  = hl_proteins[[map_id]];
    const kegg_map  = KEGG_maps[[map_id]];

    cat(`process ${map_id}, has ${length(compounds) + length(genes) + length(proteins)} objects for highlights...`);

    try({
        # do pathway map highlights rendering
        kegg_map
        |> html(compounds = compounds, genes = genes, proteins = proteins)
        |> writeLines(
            con = `${outputdir}/${map_id}.html`
        );

        bitmap(file = `${outputdir}/${map_id}.png`) {
            plot(kegg_map, compounds = compounds, genes = genes, proteins = proteins);
        }
    });

    cat("  ~ done!\n");
}

print("highlight rendering for all kegg pathway map job done!");
