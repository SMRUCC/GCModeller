imports "kegg.repository" from "kegg_kit.dll" 

let work.dir as string = !script$dir;
let url as string = "http://www.kegg.jp/kegg/catalog/org_list.html";

url 
:> fetch.kegg_organism( type = 1) 
:> save.kegg_organism(file=`${work.dir}/kegg_prokaryote.csv`);
