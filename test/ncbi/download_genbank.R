require(GCModeller);

# https://www.ncbi.nlm.nih.gov/datasets/taxonomy/511145/

options(http.cache_dir = @dir);

assm_list = reference_genome(ncbi_taxid = 511145);
assm_list = first(assm_list);

str(assm_list);

let id = assm_list$assembly$assembly_accession;

str(id);


let gb = fetch_genbank(accession_id = id);

print(gb);