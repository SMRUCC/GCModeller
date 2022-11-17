imports "package_utils" from "devkit";
imports "http" from "webKit";

package_utils::attach("D:\\GCModeller\\src\\workbench\\pkg");
package_utils::attach("D:\\GCModeller\\src\\repository\\graphquery\\kegg");

require(kegg_graphquery);
require(HDS);
require(GCModeller);

const cache_dir = [?"--cache" || stop("No data cahce file!")] |> http::http.cache();
const Tcode     =  ?"--tcode" || "map";
const pathways  = list_pathway(Tcode, cache = cache_dir);

for(name in names(pathways)) {    
    print(`${name}: ${pathways[[name]]}`);

    name 
    |> kegg_pathway(cache = cache_dir)
    |> xml()
    |> writeLines(`/pathways/${name}.xml`, fs = cache_dir)
    ;
}

close(cache_dir);