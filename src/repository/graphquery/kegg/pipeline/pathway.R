imports "package_utils" from "devkit";
imports "http" from "webKit";

package_utils::attach("D:\\GCModeller\\src\\workbench\\pkg");
package_utils::attach("D:\\GCModeller\\src\\repository\\graphquery\\kegg");

require(kegg_graphquery);
require(HDS);

const cache_dir = [?"--cache" || stop("No data cahce file!")] |> http::http.cache();
const Tcode     = ?"--tcode" || "map";
const pathways  = list_pathway(Tcode);

for(name in names(pathways)) {
    print(`${name}: ${pathways[[name]]}`);

}