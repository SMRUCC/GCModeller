#' visual plot of the multiple omics union data
#' 
#' @param union_data should be a dataframe value that contains the map id for make pathway map rendering and also the corrsponding highlight information for make render.
#'     the highlight information for the compound/genes/proteins is specific by the column name which is input from the parameter: compound, gene, protein.
#' @param id specific the column name of the kegg id in the input dataframe
#' @param compound specific the column name of the metabolite highlights information in the input dataframe
#' @param gene specific the column name of the gene highlights information in the input dataframe
#' @param protein specific the column name of the protein highliths information
#' @param text.color set the label text color on the kegg pathway map
#' @param kegg_maps should be a directory path that contains the kegg pathway map xml data files.
#'    default null means use the package internal resource file.
#' 
#' @details about the highlight information, seed the parser function [parse.highlight_tuples](https://gcmodeller.org/vignettes/kegg_kit/report.utils/parse.highlight_tuples.html). 
#' 
const union_render = function(union_data, outputdir = "./",
                              id         = "KEGG", 
                              compound   = "compound", 
                              gene       = "gene", 
                              protein    = "protein", 
                              text.color = "white",
                              kegg_maps  = NULL) {

    const KEGG_maps = __load_kegg_map(kegg_maps);

    if (is.character(union_data)) {
        union_data = read.csv(union_data, row.names = NULL, 
            check.names = FALSE);
    }

    print("view of the union data for run kegg report export:");
    print(union_data, max.print = 6);

    for(map_data in as.list(union_data, byrow = TRUE)) {
        # parse.highlight_tuples: Parse the kegg pathway node highlight information
        # corresponding map_data column value should be a character string that contains the pathway nodes id and 
        # optional highligh color for make the kegg pathway map rendering.
        # 
        # value should be in formats of:
        # 
        #     "K00001:blue;K00002:red;C00001:green"
        # 
        # where the first part is the kegg id, the second part is the highlight color.
        # 
        # or just a list of the kegg id without highlight color, such as:
        # 
        #     "K00001;K00002;C00001"
        # 
        # The default color is "red" if the highlight color is not specified. 
        let compounds = parse.highlight_tuples(map_data[[compound]]);
        let genes     = parse.highlight_tuples(map_data[[gene]]);
        let proteins  = parse.highlight_tuples(map_data[[protein]]);
        let map_id    = unify_mapid(x = map_data[[id]]);

        # compounds/genes/proteins are a tuple list that contains the highlight information, such as:
        #
        # ```r
        # list(K00001 = "blue", K00002 = "red", C00001 = "green");
        # ```

        try(ex -> {
            let template = KEGG_maps[[map_id]];
            
            template |> html( 
                compounds  = compounds, 
                genes      = genes, 
                proteins   = proteins, 
                text.color = text.color
            )
            # print html text to std_out device
            |> writeLines(con = `${outputdir}/${map_id}.html`)
            ;

            # just render image file
            bitmap(file = `${outputdir}/${map_id}.png`) {
                plot(template, 
                    compounds  = compounds, 
                    genes      = genes, 
                    proteins   = proteins, 
                    text.color = text.color);
            }
        }) {
            print(`found error while rendering ${map_id}:`);
            print([ex]::error);
        };
    }
}
