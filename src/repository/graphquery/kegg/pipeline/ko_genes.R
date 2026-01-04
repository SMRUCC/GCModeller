require(kegg_api);
require(GCModeller);
require(REnv);

imports "dbget" from "kegg_api";
imports "kegg_api" from "kegg_api";

let ko_dbfile = HDS::openStream(relative_work("ko_genes.dat"), allowCreate = TRUE, meta_size = 32[MB]);
let genomes = kegg_api::listing("organism");

genomes <- lapply(genomes, str -> strsplit(str, "\t"));
genomes <- data.frame(
    row.names = names(genomes),
    kegg_code = genomes@{1},
    organism = genomes@{2},
    kingdom = sapply(genomes@{3},str -> strsplit(str,";")[1]),
    lineage = genomes@{3}
);

print(unique(genomes$kingdom));

let bacterial = genomes[genomes$kingdom == "Prokaryotes", ];

print(bacterial, max.print = 6);

write.csv(bacterial, file = relative_work("bacterial.csv"));

kegg_api::ko_db(db = ko_dbfile, species = bacterial$kegg_code, download_seqs=FALSE);

close(ko_dbfile);