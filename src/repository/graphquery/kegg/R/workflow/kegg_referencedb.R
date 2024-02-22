
#' workflow function for create the kegg reference database
#' 
#' @param db_file a character vector of the database file its 
#'    file path. for save the kegg data models. 
#'  
const kegg_referencedb = function(db_file = "./kegg.db") {
    # load internal kegg brite database and then
    # convert as dataframe
    const brite = brite::parse("br08901");
    const brite_df = as.data.frame(brite);
    const cache_dir = db_file
    |> HDS::openStream(allowCreate = TRUE, meta_size = 32*1024*1024)
    |> http::http.cache()
    ;

    print("views of the pathway list for do query:");
    print(brite_df, max.print = 6);

    for(map in as.list(brite_df, byrow = TRUE)) {
        __query_map(map, cache_dir);
    }

    HDS::flush([cache_dir]::fs);
}

