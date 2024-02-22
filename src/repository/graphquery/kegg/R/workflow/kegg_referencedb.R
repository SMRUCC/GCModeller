
#' workflow function for create the kegg reference database
#' 
#' @param db_file a character vector of the database file its 
#'    file path. for save the kegg data models. 
#'  
const kegg_referencedb = function(db_file = "./kegg.db") {
    let brite = brite::parse("br08901");
let df = as.data.frame(brite);

const cache_dir = `${@dir}/cache.db`
|> HDS::openStream(allowCreate = TRUE, meta_size = 32*1024*1024)
|> http::http.cache()
;
const cache_fs = [cache_dir]::fs;

print(df, max.print = 6);

for(map in as.list(df, byrow = TRUE)) {

}

HDS::flush(cache_fs);
}

