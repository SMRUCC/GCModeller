require(kegg_api);
require(GCModeller);
require(REnv);

let ko_dbfile = HDS::openStream(relative_work("ko_genes.dat"), allowCreate = TRUE, meta_size = 32[MB]);

ko_db(db = ko_dbfile);

close(ko_dbfile);