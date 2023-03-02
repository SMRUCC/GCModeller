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
const enzyme_class = enzyme_description();
const class_labels = as.list(names(enzyme_class), names = `EC_${unlist(enzyme_class)}`);

str(reactions);
str(class_labels);

for(id in names(reactions)) {
    const reaction = kegg_reaction(id, cache = cache_dir);
    
    for(ec_number in [reaction]::Enzyme) {
        const tokens = unlist(strsplit(ec_number, ".", fixed = TRUE));        

        if (`EC_${tokens[1]}` in class_labels) {
            tokens[1] = class_labels[[`EC_${tokens[1]}`]];
        }

        reaction
        |> xml()
        |> writeLines(
            con = `/reactions/${paste(tokens, "/")}/${id}.xml`, 
            fs = [cache_dir]::fs
        )
        ;

        print(paste(tokens, "."));
    }

    print(reactions[[id]]);
}

close([cache_dir]::fs);