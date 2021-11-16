require(kegg_graphquery);

# request kegg compounds data models from kegg 
# that required by reaction class data models

const map_repo as string  = ?"--repo" || stop("a kegg data repository of pathway map is required!");
const saveDir as string   = ?"--save" || `${dirname(map_repo)}/maps/`;
const url_templ as string = "https://www.kegg.jp/dbget-bin/www_bget?cpd:%s";

options(http.cache_dir = ?"--cache" || `${dirname(@script)}/.cache/`);

for(let cid in load.maps(map_repo) |> compoundsId) {
    const keg_compound = kegg_compound(url = sprintf(url_templ, cid));
    const saveXML      = `${saveDir}/${cid}.XML`;
	const isNull       = is.null(keg_compound);

    if (!isNull) {
        keg_compound
        |> xml
        |> writeLines(con = saveXML)
        ;
    }
}