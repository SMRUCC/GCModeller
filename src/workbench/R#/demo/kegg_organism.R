imports "kegg.repository" from "kegg_kit.dll" 

let work.dir as string = !script$dir;
let save.file as string = `${work.dir}/kegg_prokaryote.csv`;
let url as string = ?"--url" || "http://www.kegg.jp/kegg/catalog/org_list.html";

print("Organism table file save at location:");
print(save.file);

url 
:> fetch.kegg_organism( type = 1) 
:> save.kegg_organism(file= save.file);
