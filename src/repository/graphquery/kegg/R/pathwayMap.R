imports "repository" from "kegg_kit";

const kegg_map as function(url) {
    const info  = http_query(url, raw = FALSE, graphquery = get_graph("graphquery/map_summary.graphquery"));
    const areas = http_query(url,
                             raw        = FALSE,
                             graphquery = get_graph("graphquery/kegg_map.graphquery")
    )
    |> area_table
    |> shapeAreas
    ;

    repository::keggMap(
        id   = info$id,
        name = info$name,
        img  = `https://www.kegg.jp/${info$img}`
        |> getImage
        |> base64(chunkSize = 128),
        url  = url,
        area = areas
    );
}

const area_table as function(list) {
    const fullNames = unique(unlist(lapply(list, names)));
    const tableList = lapply(fullNames, function(name) {
        sapply(list, i -> i[[name]] || "NULL");
    });

    names(tableList) = fullNames;

    const table = as.data.frame(tableList);

    print("view of the shape areas data:");
    str(table);

    table;
}
