imports ["repository", "metabolism", "report.utils"] from "kegg_kit";

#' Do rendering of the kegg pathway map
#' 
#' @param KEGG_maps the kegg maps repository data object
#' @param pathwayList the list data of the kegg url for do rendering, this
#'    parameter value should be a list of the key-value paire to the url
#'    string data.
#' 
const localRenderMap = function(KEGG_maps, pathwayList,
                                compoundcolors = "red",
                                gene_highights = "blue",
                                outputdir      = "./", 
                                min_objects    = 0) {

    # key name is the kegg pathway map id
    # key value is the kegg pathway map url
    const pathwayId as string = unify_mapid(x = names(pathwayList));
    const urlLinks as string = unlist(pathwayList);

    print("script will render target objects on kegg map:");
    print(pathwayId);
    print(outputdir);
    print(urlLinks);

    print("start rendering for each KEGG maps:");

    for(i in 1:length(pathwayId)) {
        const mapId as string = pathwayId[i];
        const url   as string = urlLinks[i];

        #' parse hightlights from url with two url schema:
        #'
        #' 1. http://www.kegg.jp/pathway/map01230+C00037+C00049+C00082+C00188
        #'
        #'    will be parsed as {id: gene_highlights/compoundcolors}
        #'
        #' 2. http://www.kegg.jp/pathway/map01230/C00037/red/C00049/blue
        #'
        #'    will be parsed as {id: color1, id: color2}
        #'
        const highlights = .safe_kegg_url_parser(url, compoundcolors, gene_highights);

        print(`'${mapId}' contains ${length(highlights$objects)} objects.`);
        print(url);

        if (length(highlights$objects) >= min_objects) {
            try(ex -> {
                KEGG_maps[[mapId]]
                |> keggMap.reportHtml(highlights$objects)
                # print html text to std_out device
                |> writeLines(con = `${outputdir}/${mapId}.html`)
                ;

                # just render image file
                bitmap(file = `${outputdir}/${mapId}.png`) {
                    keggMap.highlights(KEGG_maps[[mapId]], highlights$objects);
                }
            }) {
                print(`found error while rendering ${mapId}:`);
                print([ex]::error);
                str(highlights);
            };
        } else {
            print("skip rendering plot due to the reason of too less object.");
        }
    }
}

#' unify the kegg map id as reference map id
#' 
#' @param x the kegg map id, maybe in organism species related pathway map id, example as ``hsaxxxxxx``.
#' 
const unify_mapid = function(x) {
    `map${$"\d+"(x)
    |> unlist()
    |> as.integer()
    |> str_pad(5, "left", "0")}`
}