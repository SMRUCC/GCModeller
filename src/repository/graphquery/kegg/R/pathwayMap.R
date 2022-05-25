imports "repository" from "kegg_kit";

#' query for kegg map
#' 
const kegg_map = function(url) {
    const info  = http_query(url, raw = FALSE, graphquery = get_graph("graphquery/map_summary.graphquery"));
    const areas = http_query(url,
                             raw        = FALSE,
                             graphquery = get_graph("graphquery/kegg_map.graphquery")
    )
    |> area_table
    |> shapeAreas
    ;

    str(info);

    repository::keggMap(
        id          = info$id,
        name        = info$name,
        img         = `https://www.kegg.jp/${info$img}`
        |> getImage
        |> base64(chunkSize = 128),
        url         = url,
        area        = areas,
        description = info$description
    );
}

#' generate table data of the area shape on the map
#' 
const area_table = function(list) {
    const fullNames = unique(unlist(lapply(list, i -> names(i))));
    const tableList = lapply(fullNames, function(name) {
        sapply(list, i -> i[[name]] || "NULL");
    });

    names(tableList) = fullNames;

    const table = as.data.frame(tableList);

    print("view of the shape areas data:");
    str(table);

    table;
}
