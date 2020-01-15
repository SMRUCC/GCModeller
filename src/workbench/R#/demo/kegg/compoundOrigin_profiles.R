imports ["taxonomy_kit", "metagenomics_kit"] from "metagenomics_kit.dll";

let taxonomy.tree <- Ncbi.taxonomy_tree("S:\synthetic_biology\taxdmp");
# let test_org as string = "S:\biodeepdb\kegg\enzymes\xcb";
# let empty <- taxonomy.tree :> compounds.origin.profile(test_org);

# print(json(empty)); 

let createProfiles as function(org) {
	taxonomy.tree :> compounds.origin.profile(org);
}

let orgs <- list();
let org;

for(org.dir in list.dirs("S:\biodeepdb\kegg\enzymes")) {
	org <- createProfiles(org.dir);
	orgs[[basename(org.dir)]] <- org;
	
	print(as.object(org)$taxonomy);
}



