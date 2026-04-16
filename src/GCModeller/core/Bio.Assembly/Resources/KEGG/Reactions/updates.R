imports "http" from "webKit";

const kegg_url = "https://www.genome.jp/kegg-bin/download_htext?htext=%s.keg&format=htext&filedir=";

reactions = [
	"br08201" # Enzymatic reactions
    "br08202" # IUBMB reaction hierarchy
    "br08204" # Reaction class
    "br08203" # Glycosyltransferase reactions
];

str(reactions);

for(id in reactions) {
	const url = sprintf(kegg_url, id);
	const text = http::requests.get(url);
	
	writeLines(text, con = `${@dir}/${id}.txt`);
}