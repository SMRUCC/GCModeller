imports "GenBank" from "seqtoolkit";

# https://www.ncbi.nlm.nih.gov/datasets/taxonomy/511145/

const genbank_annotation_flags = ["GENOME_FASTA","GENOME_GFF","GENOME_GBFF","RNA_FASTA","CDS_FASTA","PROT_FASTA","SEQUENCE_REPORT"];
const fetch_genbank_api = "https://api.ncbi.nlm.nih.gov/datasets/v2alpha/genome/accession/%s/download?include_annotation_type=%s&filename=%s.zip";

const fetch_genbank = function(accession_id, 
                               annotations = [
                                   "GENOME_GBFF",
                                   "GENOME_FASTA",
                                   "GENOME_GFF",
                                   "RNA_FASTA",
                                   "CDS_FASTA",
                                   "PROT_FASTA",
                                   "SEQUENCE_REPORT"
                               ]) {

    const url = sprintf(fetch_genbank_api, accession_id, paste(annotations, ","), accession_id);
    const http.cache_dir = getOption("http.cache_dir") || stop("You should set of the 'http.cache_dir' option at first!");
    const key = md5(accession_id);
    const temp_dir = `${http.cache_dir}/${substr(key, 3,2)}/${accession_id}/`;
    const temp = `${temp_dir}/ncbi_download.zip`;
    const gbff_file = `${temp_dir}/ncbi_dataset/data/${accession_id}/genomic.gbff`;

    print(`fetch: ${url}`);
    # download the target ncbi dataset package
    if (!file.exists(temp)) {
        wget(url, saveAs = temp);
    }    
    # and then extract the zip dataset package at temp location
    unzip(temp, exdir = temp_dir);

    if (!file.exists(gbff_file)) {
        stop([
            "Missing target genomics genbank file in the result ncbi dataset!", 
            `You can check of the temp location: ${temp_dir}`
        ]);
    }    

    # return the genbank dataset
    read.genbank(file = gbff_file);
}

const fetch_reference_genome = "https://api.ncbi.nlm.nih.gov/datasets/v1/genome/taxon/%s?page_size=100&filters.reference_only=true";

#' get genbank reference genome accession id 
#' 
#' @param ncbi_taxid mapping the genbank accession id from this given ncbi
#'    taxonomy id
#' 
#' @return a list of the reference assembly data list
#' 
const reference_genome = function(ncbi_taxid) {
    const url  = sprintf(fetch_reference_genome, ncbi_taxid);
    const list = REnv::getJSON(url, interval = 0);

    return(list$assemblies);
}