require(kegg_graphquery);

options(http.cache_dir = ?"--cache" || `${dirname(@script)}/.cache/`);

const category_all  = compound_brites();
const url as string = "https://www.kegg.jp/dbget-bin/www_bget?cpd:%s";
const runQuery as function(name) {
    const class   = as.data.frame(category_all[[name]]);
    const repoDir = enumeratePath(class);
    const id      = class[, "entry"];

    print("get all kegg compound class:");
    str(class);

    for(i in 1:nrow(class)) {
		const filepath as string = `${name}/${repoDir(i)}.XML`;
		
		if (file.exists(filepath)) {
			cat(".");
		} else {
			const keg_compound = kegg_compound(url = sprintf(url, id[i]));

			if ((keg_compound != "") && (!is.null(keg_compound))) {

				keg_compound
				|> xml
				|> writeLines(con = filepath)
				;
			}
		}
    }
}

for(name in names(category_all)) {
    name 
    |> runQuery
    ;
}
