require(kegg_api);
require(GCModeller);
require(REnv);

let dbfile = "G:\GCModeller\src\repository\graphquery\kegg\pipeline\package_updater\compound_cache.db";
let stream = HDS::openStream(dbfile, readonly = TRUE);

extract_files(stream, fs ="Z:\compounds" );
