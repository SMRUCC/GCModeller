require(GCModeller);

imports "NCBI" from "annotationKit";

let index_file = ?"--index" || stop("a summary index file for download ncbi genbank is required!");
let download_dir = ?"--save_dir" || "./ncbi_genbank_ftp";
let index_data = NCBI::genome_assembly_index(index_file);

for(let ref in index_data) {
    let key = md5([ref]::assembly_accession);
    let subdir = `${substr(key,2,4)}/${substr(key,8,11)}`;
    
    ncbi_assembly_ftp(ref, `${download_dir}/${subdir}`,overrides=FALSE);
}
