#' visual plot of the multiple omics union data
#' 
const union_render = function(union_data, outputdir = "./",
                              id         = "KEGG", 
                              compound   = "compound", 
                              gene       = "gene", 
                              protein    = "protein", 
                              text.color = "white",
                              kegg_maps = NULL) {

    const KEGG_maps = __load_kegg_map(kegg_maps);

    for(map_data in as.list(union_data, byrow = TRUE)) {
        let compounds = parse.highlight_tuples(map_data[[compound]]);
        let genes     = parse.highlight_tuples(map_data[[gene]]);
        let proteins  = parse.highlight_tuples(map_data[[protein]]);
        let map_id    = unify_mapid(x = map_data[[id]]);

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
