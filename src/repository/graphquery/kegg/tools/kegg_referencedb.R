require(kegg_api);

# make kegg reference database updates
# for the gcmodeller package
kegg_api::kegg_referencedb(db_file = file.path(@dir, "kegg.db"));
