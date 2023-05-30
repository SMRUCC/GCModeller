imports "package_utils" from "devkit";

package_utils::attach(`${@dir}/../../`);

options(http.cache_dir = @dir);

tax = taxonomy_search("Magnetospirillum magneticum AMB-1");

str(tax);

assm_list = reference_genome(ncbi_taxid =tax );
assm_list = first(assm_list);

str(assm_list);

let id = assm_list$assembly$assembly_accession;

str(id);


let gb = fetch_genbank(accession_id = id);

print(gb);