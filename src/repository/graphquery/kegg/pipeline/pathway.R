imports "package_utils" from "devkit";
imports "http" from "webKit";

package_utils::attach("D:\\GCModeller\\src\\workbench\\pkg");
package_utils::attach("D:\\GCModeller\\src\\repository\\graphquery\\kegg");

require(kegg_graphquery);
require(HDS);
require(GCModeller);

const cache_dir = [?"--cache" || stop("No data cahce file!")] 
|> HDS::openStream() 
|> http::http.cache()
;
const Tcode     =  ?"--tcode" || "map";
const pathways  = list_pathway(Tcode, cache = cache_dir);

for(name in names(pathways)) {  
    const pathway = kegg_pathway(name, cache = cache_dir);
    const dir = paste([pathway]::class, "/");

    print(`${name}: ${pathways[[name]]} | ${dir}`);
    
    pathway
    |> xml()
    |> writeLines(`/pathways/${dir}/${name}.xml`, fs = cache_dir)
    ;
}

close(cache_dir);