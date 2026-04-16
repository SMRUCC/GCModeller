imports "http" from "webKit";

const kegg_url = "https://www.genome.jp/kegg-bin/download_htext?htext=%s.keg&format=htext&filedir=";

reactions = [
	"br08610"
	"br08901"
	"br08902"
	"br08907"
	"ko00000"
	"ko00001"
	"ko00002"
	"ko00003"
	"ko01000" 
];

str(reactions);

for(id in reactions) {
	const url = sprintf(kegg_url, id);
	const text = http::requests.get(url);
	
	writeLines(text, con = `${@dir}/${id}.txt`);
}