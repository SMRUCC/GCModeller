imports "http" from "webKit";

const kegg_url = "https://www.genome.jp/kegg-bin/download_htext?htext=%s.keg&format=htext&filedir=";

compounds = [
	"br08001", #  Compounds with biological roles
    "br08002", #  Lipids
    "br08003", #  Phytochemical compounds
    "br08005", #  Bioactive peptides
    "br08006", #  Endocrine disrupting compounds
    "br08007", #  Pesticides
	"br08008", #  Carcinogens
    "br08009", #  Natural toxins
	"br08010", #  Target-based Classification of Compounds
    "br08021"  #  Glycosides
];

str(compounds);

for(id in compounds) {
	const url = sprintf(kegg_url, id);
	const text = http::requests.get(url);
	
	writeLines(text, con = `${@dir}/${id}.txt`);
}