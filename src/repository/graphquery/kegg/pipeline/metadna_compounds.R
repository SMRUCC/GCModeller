require(kegg_graphquery);

# request kegg compounds data models from kegg 
# that required by reaction class data models

const rxn_repo as string  = ?"--repo" || stop("a kegg reaction class data repository is required!");
const saveDir as string   = ?"--save" || `${dirname(rxn_repo)}/reaction_class/`;
const url_templ as string = "https://www.kegg.jp/dbget-bin/www_bget?cpd:%s";

options(http.cache_dir = ?"--cache" || `${dirname(@script)}/.cache/`);

for(let cid in reaction_class.repo(rxn_repo) |> compoundsId) {
    const keg_compound = kegg_compound(url = sprintf(url_templ, cid));
    const saveXML      = `${saveDir}/${cid}.XML`;

    if ((keg_compound != "") && (!is.null(keg_compound))) {
        keg_compound
        |> xml
        |> writeLines(con = saveXML)
        ;
    }
}