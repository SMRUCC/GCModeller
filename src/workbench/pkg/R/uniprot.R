const download_proteins = function(q, tax_id = NULL, as_fasta = TRUE) {
    imports "UniProt" from "annotationKit";

    let result = UniProt::rest_query(q, tax_id);

    if (as_fasta) {
        UniProt::extract_fasta(result);
    } else {
        result;
    }
}