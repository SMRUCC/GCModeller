require(kegg_api);
require(HDS);
require(GCModeller);

imports "package_utils" from "devkit";
imports "http" from "webKit";

const cache_dir = [?"--cache" || stop("No data cahce file!")] 
|> HDS::openStream(allowCreate = TRUE)
|> http::http.cache()
;
const reactions = kegg_api::listing("reaction", cache = cache_dir);

str(reactions);

for(id in names(reactions)) {
    const reaction = kegg_reaction(id, cache = cache_dir);
    
    for(ec_number in [reaction]::Enzyme) {
        reaction
        |> xml()
        |> writeLines(
            con = `/reactions/${gsub(ec_number, ".", "/")}/${id}.xml`, 
            fs = [cache_dir]::fs
        )
        ;

        print(ec_number);
    }

    print(reactions[[id]]);
}

close([cache_dir]::fs);