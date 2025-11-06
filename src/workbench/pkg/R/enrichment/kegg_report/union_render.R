#' Render KEGG Pathway Maps with Multi-Omics Highlighting
#'
#' Generates interactive HTML and static PNG visualizations of KEGG pathway maps 
#' with customizable highlighting for compounds, genes, and proteins. Output files 
#' are saved to the specified directory.
#'
#' @param union_data A data frame or file path (CSV) containing KEGG pathway IDs 
#'        and highlight information. Must include columns specified in `id`, `compound`, 
#'        `gene`, and `protein` parameters. If a character string is provided, 
#'        it is interpreted as a file path to a CSV file. 
#' @param outputdir Output directory path for rendered files (default: "./").
#' @param id Column name in `union_data` containing KEGG pathway IDs (default: "KEGG").
#' @param compound Column name in `union_data` containing compound highlight data 
#'        (default: "compound"). See Details for formatting.
#' @param gene Column name in `union_data` containing gene highlight data 
#'        (default: "gene").
#' @param protein Column name in `union_data` containing protein highlight data 
#'        (default: "protein").
#' @param text.color Color for node labels on pathway maps (default: "white").
#' @param kegg_maps Directory path containing KEGG map XML files. If `NULL` (default), 
#'        uses the package's internal XML resources. 
#'
#' @details 
#' 
#' **Highlight Data Format**:  
#' 
#' The `compound`, `gene`, and `protein` columns should contain strings in one of two formats:  
#' 
#' 1. Semicolon-separated key-value pairs: `"K00001:blue;K00002:red;C00001:green"`  
#' 2. Semicolon-separated IDs (default color: red): `"K00001;K00002;C00001"`  
#' 
#' These are parsed by [`parse.highlight_tuples()`](https://gcmodeller.org/vignettes/kegg_kit/report.utils/parse.highlight_tuples.html) 
#' into named lists (e.g., `list(K00001 = "blue", K00002 = "red")`). 
#' 
#' **Output Files**:  
#' 
#' For each KEGG ID, two files are generated:  
#' 
#' - `[KEGG_ID].html`: Interactive pathway visualization  
#' - `[KEGG_ID].png`: Static PNG image  
#' 
#' **Error Handling**:  
#' 
#' Errors during rendering are caught and printed to the console without stopping execution.
#' 
#' @return Invisibly returns `NULL`. Primary output is generated as files in `outputdir`.
#' @export
#' @examples
#' \dontrun{
#' # Minimal example with default column names
#' data <- data.frame(
#'   KEGG = "map00010",
#'   compound = "C00031:green;C00022:blue",
#'   gene = "hsa:1234;hsa:5678:yellow",
#'   protein = "P12345"
#' )
#' union_render(data, outputdir = "~/kegg_output")
#'
#' # Custom column names and text color
#' data <- data.frame(
#'   PathwayID = "map00020",
#'   Metabolites = "C00024; C00036:orange",
#'   Genes = "hsa:9012:purple",
#'   Proteins = "P67890:red"
#' )
#' union_render(data,
#'   id = "PathwayID",
#'   compound = "Metabolites",
#'   gene = "Genes",
#'   protein = "Proteins",
#'   text.color = "black"
#' )}
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
