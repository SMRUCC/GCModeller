imports "http" from "webKit";

let kegg_url = "https://www.genome.jp/kegg-bin/download_htext?htext=%s.keg&format=htext&filedir=";
let reactions = [
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

for(let id in reactions) {
	sprintf(kegg_url, id)
	|> http::requests.get()
	|> writeLines(con = here(`${id}.txt`))
	;
}

"https://rest.kegg.jp/list/ko"
|> http::requests.get()
|> writeLines(con = here("ko.txt"))
;