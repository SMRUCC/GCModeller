imports ["taxonomy_kit", "metagenomics_kit"] from "metagenomics_kit.dll";

# "S:\biodeepdb\kegg\test"
let dirs as string <- list.dirs("S:\biodeepdb\kegg\enzymes", fullNames = TRUE, recursive = FALSE);
let taxonomy.tree <- Ncbi.taxonomy_tree("S:\synthetic_biology\taxdmp");
# let test_org as string = "S:\biodeepdb\kegg\enzymes\xcb";
# let empty <- taxonomy.tree :> compounds.origin.profile(test_org);

# print(json(empty));

print(basename(dirs));

let createProfiles as function(org) {
	taxonomy.tree :> compounds.origin.profile(org);
}

let orgs <- list();
let org;

# "S:\biodeepdb\kegg\enzymes"
for(org.dir in dirs :> which( d -> file.exists(`${d}/kegg_compounds.txt`) )) {
	org <- createProfiles(org.dir);
	orgs[[basename(org.dir)]] <- org;
	
	print(as.object(org)$taxonomy);
}

orgs 
:> unlist(valueof compounds.origin.profile) 
:> write.csv(file = "S:\biodeepdb\kegg\compounds.origins.csv");

