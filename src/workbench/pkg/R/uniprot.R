#' Fetch protein data from uniprot via rest api
#' 
#' @param q the query term
#' 
const download_proteins = function(q, tax_id = NULL, as_fasta = TRUE) {
    imports "UniProt" from "annotationKit";

    let result = UniProt::rest_query(q, tax_id);

    if (as_fasta) {
        UniProt::extract_fasta(result);
    } else {
        result;
    }
}

#' make kegg gsea background via the link ko request result and uniprot protein table export data
#' 
#' @param proteinTable the uniprot protein table export via the uniprot tool function ``proteinTable``.
#' @param ko_maps a dataframe of the ko_maps with data fields: kegg_id and KO. or a directory path 
#'    as the local repository that contains the ko link result by request the kegg rest api via ``link_ko`` function, 
#'    example as:
#'    https://rest.kegg.jp/link/ko/taes:803091+taes:803092+taes:123456
#' 
const uniprot_background = function(proteinTable, ko_maps, id_key = "row.names") {
    imports "background" from "gseakit";

    if (!is.data.frame(ko_maps)) {
        ko_maps = list.files(ko_maps, pattern = "*.txt");
        ko_maps = lapply(ko_maps, file => {
            let link_ko = read.table(
                file, row.names = NULL, header = FALSE, 
                check.names= FALSE, 
                sep = "\t"
            );
            colnames(link_ko) = c("kegg_id","KO");
            link_ko;
        });
        ko_maps = bind_rows(ko_maps);
    }

    let ko = background::KO_reference();
    let koSet = as.geneSet(ko);

    message("inspect of the reference KO set:");
    str(koSet);

    ko_maps = ko 
    |> group_by("KO") 
    |> lapply(link => link$kegg_id)
    ;

    message("inspect of the gene id to ko mapping:");
    str(ko_maps);

    proteinTable = as.data.frame(proteinTable);
    
    if (id_key = "row.names" || id_key == "0") {
        id_key = "id_key";
        proteinTable[,"id_key"] = rownames(proteinTable);
    }

    proteinTable = proteinTable |> group_by(id_key);

    let clusters = as.list(names(koSet), names = names(koSet)) 
    |> lapply(function(map_id) {
        let map_info = tagvalue(map_id, "-", as.list = TRUE);
        let ko_ids = koSet[[names(map_info)]];
        let geneset = ko_maps[ko_ids];

        if (length(geneset) == 0) {
            return(NULL);
        } else {
            geneset = proteinTable[geneset];
            geneset = bind_rows(geneset);
            geneset = data.frame(
                xref = geneset[,id_key];
                name = geneset$name,
                alias = geneset$geneName,
                KEGG = geneset$KEGG,
                uniprot = rownames(geneset)
            );

            return(gsea_cluster(
                x = geneset,
                clusterId = names(map_info), 
                clusterName = unlist(map_info)
            ));
        }
    }) |> which(c -> !is.null(c));

    # cast the gene cluster collection to gsea background model
    as.background(clusters);
}

