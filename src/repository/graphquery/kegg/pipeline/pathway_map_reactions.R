require(GCModeller);
require(kegg_api);
require(HDS);

imports "package_utils" from "devkit";
imports "http" from "webKit";

maps = kegg_maps(raw = TRUE);
shapes = unlist([maps]::shapes);
idset = unlist([shapes]::IDVector) |> unique();

str(shapes);


print(idset);

reactions = idset[idset == $"R\d+"];

print(reactions);

const cache_dir = [?"--cache" || stop("No data cahce file!")] 
|> HDS::openStream(allowCreate = TRUE)
|> http::http.cache()
;
const enzyme_class = enzyme_description();
const class_labels = as.list(names(enzyme_class), names = `EC_${unlist(enzyme_class)}`);

for(id in reactions) {
    const reaction = kegg_reaction(id, cache = cache_dir);
    const ec_numbers = [reaction]::Enzyme;

    for(ec_number in ec_numbers) {
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