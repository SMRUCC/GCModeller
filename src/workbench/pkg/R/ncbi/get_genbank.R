imports "GenBank" from "seqtoolkit";

const genbank_annotation_flags = ["GENOME_FASTA","GENOME_GFF","RNA_FASTA","CDS_FASTA","PROT_FASTA","SEQUENCE_REPORT"];
const fetch_genbank_api = "https://api.ncbi.nlm.nih.gov/datasets/v2alpha/genome/accession/%s/download?include_annotation_type=%s&filename=%s.zip";

const fetch_genbank = function(accession_id, annotations = genbank_annotation_flags) {
    const url = sprintf(fetch_genbank_api, accession_id, paste(annotations, ","), accession_id);
    const http.cache_dir = getOption("http.cache_dir") || stop("You should set of the 'http.cache_dir' option at first!");
    const key = md5(accession_id);
    const temp = `${http.cache_dir}/${substr(key, 3,2)}/${accession_id}.zip`;

    wget(url, save = temp);
    unzip(temp);

    
}