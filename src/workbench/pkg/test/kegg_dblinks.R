require(GCModeller);

let cpds = kegg_compounds(rawList = TRUE, reference_set = TRUE);
let dblinks = unlist([cpds]::DbLinks);
let dbnames = [dblinks]::DBName;

print(unique(dbnames));